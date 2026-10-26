
pipeline {
    agent any

    options {
        disableConcurrentBuilds()
        timeout(time: 45, unit: 'MINUTES')

        buildDiscarder(logRotator(
            numToKeepStr: '10',
            daysToKeepStr: '14',
            artifactNumToKeepStr: '5'
        ))
    }

    environment {
        AWS_REGION         = 'ap-southeast-2'
        AWS_ACCOUNT_ID     = '730132962765'
        ECR_REGISTRY       = '730132962765.dkr.ecr.ap-southeast-2.amazonaws.com'
        BACKEND_INSTANCE_ID = 'i-07736afe0c5fca9d1'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Check Jenkins Disk Space') {
            steps {
                sh '''
                    set -eu

                    AVAILABLE_KB=$(df -Pk / | awk 'NR==2 {print $4}')
                    REQUIRED_KB=2097152

                    echo "Available disk space: ${AVAILABLE_KB} KB"

                    if [ "$AVAILABLE_KB" -lt "$REQUIRED_KB" ]; then
                        echo "ERROR: Less than 2 GB free on Jenkins root disk."
                        exit 1
                    fi

                    docker info >/dev/null
                    aws --version
                    python3 --version
                '''
            }
        }

        stage('Build and Push Images') {
            steps {
                sh '''
                    set -eu

                    echo "Authenticating Jenkins Docker client with ECR..."

                    aws ecr get-login-password \
                      --region "$AWS_REGION" \
                      | docker login \
                          --username AWS \
                          --password-stdin "$ECR_REGISTRY"

                    build_and_push() {
                        SERVICE="$1"
                        DOCKERFILE="$2"
                        IMAGE="$ECR_REGISTRY/eventflow-$SERVICE"

                        if [ ! -f "$DOCKERFILE" ]; then
                            echo "ERROR: Dockerfile not found: $DOCKERFILE"
                            exit 1
                        fi

                        echo "Building eventflow-$SERVICE"

                        docker build \
                          --pull \
                          -f "$DOCKERFILE" \
                          -t "$IMAGE:$BUILD_NUMBER" \
                          -t "$IMAGE:latest" \
                          .

                        docker push "$IMAGE:$BUILD_NUMBER"
                        docker push "$IMAGE:latest"
                    }

                    build_and_push gateway Dockerfile.gateway
                    build_and_push identity Dockerfile.identity
                    build_and_push event Dockerfile.event
                    build_and_push registration Dockerfile.registration
                    build_and_push operations Dockerfile.operations
                '''
            }
        }

        stage('Deploy to Backend via SSM') {
            steps {
                sh '''
                    set -eu

                    echo "Deploying EventFlow build $BUILD_NUMBER via SSM"
                    echo "Backend instance: $BACKEND_INSTANCE_ID"

                    COMPOSE_B64=$(base64 docker-compose.prod.yml | tr -d '\n')
                    export COMPOSE_B64

                    export REMOTE_SCRIPT
                    REMOTE_SCRIPT=$(cat <<REMOTE
set -eu

echo "Authenticating Docker with Amazon ECR..."
aws ecr get-login-password --region ${AWS_REGION} \
  | docker login \
      --username AWS \
      --password-stdin ${ECR_REGISTRY}

echo "Checking deployment directory..."
cd /opt/eventflow

echo "Writing the repository compose file..."
printf '%s' "$COMPOSE_B64" | base64 -d > /opt/eventflow/docker-compose.prod.yml

echo "Validating production public URLs..."
if ! grep -Eq '^FRONTEND_BASE_URL=https://eventflow-web\.duckdns\.org/?$' /opt/eventflow/.env.prod; then
  echo "ERROR: FRONTEND_BASE_URL in /opt/eventflow/.env.prod must be https://eventflow-web.duckdns.org."
  exit 1
fi

if ! grep -Eq '^CORS_ORIGIN_0=https://eventflow-web\.duckdns\.org/?$' /opt/eventflow/.env.prod; then
  echo "ERROR: CORS_ORIGIN_0 in /opt/eventflow/.env.prod must be https://eventflow-web.duckdns.org."
  exit 1
fi

if docker-compose \
  --env-file /opt/eventflow/.env.prod \
  -f /opt/eventflow/docker-compose.prod.yml \
  config | grep -Eq 'Frontend__BaseUrl:.*(localhost|127\.0\.0\.1)'; then
  echo "ERROR: The resolved Frontend__BaseUrl points to localhost."
  exit 1
fi

if docker-compose \
  --env-file /opt/eventflow/.env.prod \
  -f /opt/eventflow/docker-compose.prod.yml \
  config | grep -Eq 'Cors__AllowedOrigins__[0-9]+:.*(localhost|127\.0\.0\.1)'; then
  echo "ERROR: A resolved CORS origin points to localhost."
  exit 1
fi

export IMAGE_TAG=${BUILD_NUMBER}

echo "Pulling build ${BUILD_NUMBER} images..."
docker-compose \
  --env-file /opt/eventflow/.env.prod \
  -f /opt/eventflow/docker-compose.prod.yml \
  pull gateway identity event registration operations

echo "Starting updated services..."
docker-compose \
  --env-file /opt/eventflow/.env.prod \
  -f /opt/eventflow/docker-compose.prod.yml \
  up -d --no-build gateway identity event registration operations

echo "Current container status:"
docker-compose \
  --env-file /opt/eventflow/.env.prod \
  -f /opt/eventflow/docker-compose.prod.yml \
  ps
REMOTE
)

                    python3 -c '
import json, os
print(json.dumps({"commands": [os.environ["REMOTE_SCRIPT"]]}))
' > ssm-deploy-commands.json

                    COMMAND_ID=$(aws ssm send-command \
                      --region "$AWS_REGION" \
                      --instance-ids "$BACKEND_INSTANCE_ID" \
                      --document-name "AWS-RunShellScript" \
                      --comment "EventFlow deployment build $BUILD_NUMBER" \
                      --parameters file://ssm-deploy-commands.json \
                      --query 'Command.CommandId' \
                      --output text)

                    echo "SSM Command ID: $COMMAND_ID"

                    for i in $(seq 1 60); do
                        STATUS=$(aws ssm get-command-invocation \
                          --region "$AWS_REGION" \
                          --command-id "$COMMAND_ID" \
                          --instance-id "$BACKEND_INSTANCE_ID" \
                          --query 'Status' \
                          --output text 2>/dev/null || true)

                        STATUS="${STATUS:-Pending}"
                        echo "Deployment status: $STATUS"

                        case "$STATUS" in
                            Success)
                                aws ssm get-command-invocation \
                                  --region "$AWS_REGION" \
                                  --command-id "$COMMAND_ID" \
                                  --instance-id "$BACKEND_INSTANCE_ID" \
                                  --query 'StandardOutputContent' \
                                  --output text
                                exit 0
                                ;;
                            Failed|Cancelled|TimedOut|Undeliverable|Terminated|InvalidPlatform)
                                aws ssm get-command-invocation \
                                  --region "$AWS_REGION" \
                                  --command-id "$COMMAND_ID" \
                                  --instance-id "$BACKEND_INSTANCE_ID" \
                                  --output json || true
                                exit 1
                                ;;
                        esac

                        sleep 5
                    done

                    echo "ERROR: Timed out waiting for deployment."
                    exit 1
                '''
            }
        }

        stage('Verify Deployment') {
            steps {
                sh '''
                    set -eu

                    echo "Checking containers and gateway API response..."

                    export REMOTE_SCRIPT
                    REMOTE_SCRIPT=$(cat <<'REMOTE'
set -eu

cd /opt/eventflow

docker-compose \
  --env-file /opt/eventflow/.env.prod \
  -f /opt/eventflow/docker-compose.prod.yml \
  ps

FRONTEND_ENV=$(docker inspect eventflow-operations \
  --format '{{range .Config.Env}}{{println .}}{{end}}' \
  | grep '^Frontend__BaseUrl=' || true)

echo "Operations $FRONTEND_ENV"

case "$FRONTEND_ENV" in
    Frontend__BaseUrl=https://eventflow-web.duckdns.org|Frontend__BaseUrl=https://eventflow-web.duckdns.org/)
        ;;
    *)
        echo "ERROR: Operations Frontend__BaseUrl is missing or incorrect."
        exit 1
        ;;
esac

for i in 1 2 3 4 5 6; do
    CODE=$(curl -sS -o /dev/null -w '%{http_code}' \
        --max-time 10 \
        http://127.0.0.1:5000/api/v1/auth/profile) || CODE=000

    case "$CODE" in
        200|401|403)
            echo "Gateway responded successfully with HTTP $CODE"
            exit 0
            ;;
    esac

    echo "Gateway not ready: HTTP $CODE"
    sleep 10
done

echo "ERROR: Gateway health verification failed."
exit 1
REMOTE
)

                    python3 -c '
import json, os
print(json.dumps({"commands": [os.environ["REMOTE_SCRIPT"]]}))
' > ssm-verify-commands.json

                    COMMAND_ID=$(aws ssm send-command \
                      --region "$AWS_REGION" \
                      --instance-ids "$BACKEND_INSTANCE_ID" \
                      --document-name "AWS-RunShellScript" \
                      --comment "Verify EventFlow build $BUILD_NUMBER" \
                      --parameters file://ssm-verify-commands.json \
                      --query 'Command.CommandId' \
                      --output text)

                    echo "Verification Command ID: $COMMAND_ID"

                    for i in $(seq 1 30); do
                        STATUS=$(aws ssm get-command-invocation \
                          --region "$AWS_REGION" \
                          --command-id "$COMMAND_ID" \
                          --instance-id "$BACKEND_INSTANCE_ID" \
                          --query 'Status' \
                          --output text 2>/dev/null || true)

                        STATUS="${STATUS:-Pending}"
                        echo "Verification status: $STATUS"

                        case "$STATUS" in
                            Success)
                                aws ssm get-command-invocation \
                                  --region "$AWS_REGION" \
                                  --command-id "$COMMAND_ID" \
                                  --instance-id "$BACKEND_INSTANCE_ID" \
                                  --query 'StandardOutputContent' \
                                  --output text
                                exit 0
                                ;;
                            Failed|Cancelled|TimedOut|Undeliverable|Terminated|InvalidPlatform)
                                aws ssm get-command-invocation \
                                  --region "$AWS_REGION" \
                                  --command-id "$COMMAND_ID" \
                                  --instance-id "$BACKEND_INSTANCE_ID" \
                                  --output json || true
                                exit 1
                                ;;
                        esac

                        sleep 5
                    done

                    echo "ERROR: Timed out waiting for verification."
                    exit 1
                '''
            }
        }
    }

    post {
        success {
            echo 'EventFlow images pushed, backend deployed, and gateway verified.'
        }

        failure {
            echo 'Pipeline failed. Check Console Output and the associated SSM command output.'
        }

        always {
            sh '''
                set +e

                echo "Docker usage before cleanup:"
                docker system df

                echo "Removing unused Docker build cache..."
                docker builder prune -af

                echo "Removing unused Docker images..."
                docker image prune -af

                echo "Docker usage after cleanup:"
                docker system df

                echo "Jenkins disk usage:"
                df -h /
            '''

            echo 'EventFlow CI/CD pipeline finished.'
        }
    }
}
