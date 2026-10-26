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

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Preflight') {
            steps {
                sh '''
                    set -eu
                    AVAILABLE_KB=$(df -Pk / | awk 'NR==2 {print $4}')
                    REQUIRED_KB=2097152
                    echo "Available Jenkins disk space: ${AVAILABLE_KB} KB"

                    if [ "$AVAILABLE_KB" -lt "$REQUIRED_KB" ]; then
                        echo "ERROR: Jenkins needs at least 2 GB free disk space."
                        exit 1
                    fi

                    docker info >/dev/null
                    aws --version
                    python3 --version

                    for file in Dockerfile.gateway Dockerfile.identity Dockerfile.event Dockerfile.registration Dockerfile.operations docker-compose.prod.yml; do
                        if [ ! -f "$file" ]; then
                            echo "ERROR: Required repository file missing: $file"
                            exit 1
                        fi
                    done
                '''
            }
        }

        stage('Build and Push Images') {
            steps {
                sh '''
                    set -eu
                    aws ecr get-login-password --region "$AWS_REGION" \
                      | docker login --username AWS --password-stdin "$ECR_REGISTRY"

                    build_and_push() {
                        SERVICE="$1"
                        DOCKERFILE="$2"
                        IMAGE="$ECR_REGISTRY/eventflow-$SERVICE"
                        echo "Building $IMAGE:$BUILD_NUMBER from $DOCKERFILE"

                        docker build --pull -f "$DOCKERFILE" \
                          -t "$IMAGE:$BUILD_NUMBER" \
                          -t "$IMAGE:latest" .
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

        stage('Deploy via AWS Systems Manager') {
            steps {
                sh '''
                    set -eu
                    echo "Preparing deployment script for build $BUILD_NUMBER..."

                    COMPOSE_B64=$(base64 docker-compose.prod.yml | tr -d '\\n')
                    export COMPOSE_B64 BUILD_NUMBER AWS_REGION ECR_REGISTRY

                    cat > remote-deploy.template.sh <<'REMOTE_SCRIPT'
#!/bin/bash
set -euo pipefail

AWS_REGION="__AWS_REGION__"
ECR_REGISTRY="__ECR_REGISTRY__"
IMAGE_TAG="__BUILD_NUMBER__"
COMPOSE_B64="__COMPOSE_B64__"

echo "Logging in to Amazon ECR..."
aws ecr get-login-password --region "$AWS_REGION" \
  | docker login --username AWS --password-stdin "$ECR_REGISTRY"

cd /opt/eventflow
if [ ! -f /opt/eventflow/.env.prod ]; then
  echo "ERROR: /opt/eventflow/.env.prod was not found."
  exit 1
fi

echo "Validating production frontend URL configuration..."
if ! grep -Eq '^FRONTEND_BASE_URL=https://eventflow-web[.]duckdns[.]org/?$' /opt/eventflow/.env.prod; then
  echo "ERROR: FRONTEND_BASE_URL must be https://eventflow-web.duckdns.org"
  exit 1
fi

if ! grep -Eq '^CORS_ORIGIN_0=https://eventflow-web[.]duckdns[.]org/?$' /opt/eventflow/.env.prod; then
  echo "ERROR: CORS_ORIGIN_0 must be https://eventflow-web.duckdns.org"
  exit 1
fi

echo "Updating docker-compose.prod.yml..."
printf '%s' "$COMPOSE_B64" | base64 -d > /opt/eventflow/docker-compose.prod.yml

if docker-compose --env-file /opt/eventflow/.env.prod \
  -f /opt/eventflow/docker-compose.prod.yml config \
  | grep -Eq 'Frontend__BaseUrl:.*(localhost|127[.]0[.]0[.]1)'; then
  echo "ERROR: Resolved Frontend__BaseUrl points to localhost."
  exit 1
fi

if docker-compose --env-file /opt/eventflow/.env.prod \
  -f /opt/eventflow/docker-compose.prod.yml config \
  | grep -Eq 'Cors__AllowedOrigins__[0-9]+:.*(localhost|127[.]0[.]0[.]1)'; then
  echo "ERROR: A resolved CORS origin points to localhost."
  exit 1
fi

export IMAGE_TAG
echo "Pulling images for build $IMAGE_TAG..."
docker-compose --env-file /opt/eventflow/.env.prod \
  -f /opt/eventflow/docker-compose.prod.yml \
  pull gateway identity event registration operations

echo "Starting updated services..."
docker-compose --env-file /opt/eventflow/.env.prod \
  -f /opt/eventflow/docker-compose.prod.yml \
  up -d --no-build gateway identity event registration operations

echo "Container status:"
docker-compose --env-file /opt/eventflow/.env.prod \
  -f /opt/eventflow/docker-compose.prod.yml ps
REMOTE_SCRIPT

                    python3 - <<'PY'
import json
import os
from pathlib import Path

script = Path("remote-deploy.template.sh").read_text()
for token, value in {
    "__AWS_REGION__": os.environ["AWS_REGION"],
    "__ECR_REGISTRY__": os.environ["ECR_REGISTRY"],
    "__BUILD_NUMBER__": os.environ["BUILD_NUMBER"],
    "__COMPOSE_B64__": os.environ["COMPOSE_B64"],
}.items():
    script = script.replace(token, value)

Path("ssm-deploy-commands.json").write_text(
    json.dumps({"commands": [script]})
)
PY

                    COMMAND_ID=$(aws ssm send-command \
                      --region "$AWS_REGION" \
                      --instance-ids "$BACKEND_INSTANCE_ID" \
                      --document-name "AWS-RunShellScript" \
                      --comment "EventFlow deployment build $BUILD_NUMBER" \
                      --parameters file://ssm-deploy-commands.json \
                      --query 'Command.CommandId' --output text)

                    echo "SSM deployment command: $COMMAND_ID"

                    for i in $(seq 1 60); do
                        STATUS=$(aws ssm get-command-invocation \
                          --region "$AWS_REGION" --command-id "$COMMAND_ID" \
                          --instance-id "$BACKEND_INSTANCE_ID" \
                          --query 'Status' --output text 2>/dev/null || true)
                        STATUS="${STATUS:-Pending}"
                        echo "Deployment status: $STATUS"

                        case "$STATUS" in
                            Success)
                                aws ssm get-command-invocation \
                                  --region "$AWS_REGION" --command-id "$COMMAND_ID" \
                                  --instance-id "$BACKEND_INSTANCE_ID" \
                                  --query 'StandardOutputContent' --output text
                                exit 0 ;;
                            Failed|Cancelled|TimedOut|Undeliverable|Terminated|InvalidPlatform)
                                aws ssm get-command-invocation \
                                  --region "$AWS_REGION" --command-id "$COMMAND_ID" \
                                  --instance-id "$BACKEND_INSTANCE_ID" \
                                  --output json || true
                                exit 1 ;;
                        esac
                        sleep 5
                    done

                    echo "ERROR: Timed out waiting for deployment command."
                    exit 1
                '''
            }
        }

        stage('Verify Deployment') {
            steps {
                sh '''
                    set -eu

                    cat > remote-verify.sh <<'REMOTE_SCRIPT'
#!/bin/bash
set -euo pipefail
cd /opt/eventflow

docker-compose --env-file /opt/eventflow/.env.prod \
  -f /opt/eventflow/docker-compose.prod.yml ps

FRONTEND_ENV=$(docker inspect eventflow-operations \
  --format '{{range .Config.Env}}{{println .}}{{end}}' \
  | grep '^Frontend__BaseUrl=' || true)
echo "Operations environment: ${FRONTEND_ENV:-not found}"

case "$FRONTEND_ENV" in
  Frontend__BaseUrl=https://eventflow-web.duckdns.org|Frontend__BaseUrl=https://eventflow-web.duckdns.org/)
    ;;
  *)
    echo "ERROR: Operations Frontend__BaseUrl is missing or incorrect."
    exit 1
    ;;
esac

for attempt in 1 2 3 4 5 6; do
  CODE=$(curl -sS -o /dev/null -w '%{http_code}' \
    --max-time 10 http://127.0.0.1:5000/api/v1/auth/profile) || CODE=000
  case "$CODE" in
    200|401|403)
      echo "Gateway verification succeeded with HTTP $CODE."
      exit 0 ;;
  esac
  echo "Gateway not ready: HTTP $CODE (attempt $attempt/6)"
  sleep 10
done

echo "ERROR: Gateway verification failed. Check gateway and downstream service logs."
exit 1
REMOTE_SCRIPT

                    python3 - <<'PY'
import json
from pathlib import Path

Path("ssm-verify-commands.json").write_text(
    json.dumps({"commands": [Path("remote-verify.sh").read_text()]})
)
PY

                    COMMAND_ID=$(aws ssm send-command \
                      --region "$AWS_REGION" \
                      --instance-ids "$BACKEND_INSTANCE_ID" \
                      --document-name "AWS-RunShellScript" \
                      --comment "Verify EventFlow build $BUILD_NUMBER" \
                      --parameters file://ssm-verify-commands.json \
                      --query 'Command.CommandId' --output text)

                    echo "SSM verification command: $COMMAND_ID"

                    for i in $(seq 1 30); do
                        STATUS=$(aws ssm get-command-invocation \
                          --region "$AWS_REGION" --command-id "$COMMAND_ID" \
                          --instance-id "$BACKEND_INSTANCE_ID" \
                          --query 'Status' --output text 2>/dev/null || true)
                        STATUS="${STATUS:-Pending}"
                        echo "Verification status: $STATUS"

                        case "$STATUS" in
                            Success)
                                aws ssm get-command-invocation \
                                  --region "$AWS_REGION" --command-id "$COMMAND_ID" \
                                  --instance-id "$BACKEND_INSTANCE_ID" \
                                  --query 'StandardOutputContent' --output text
                                exit 0 ;;
                            Failed|Cancelled|TimedOut|Undeliverable|Terminated|InvalidPlatform)
                                aws ssm get-command-invocation \
                                  --region "$AWS_REGION" --command-id "$COMMAND_ID" \
                                  --instance-id "$BACKEND_INSTANCE_ID" \
                                  --output json || true
                                exit 1 ;;
                        esac
                        sleep 5
                    done

                    echo "ERROR: Timed out waiting for verification command."
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
            echo 'Pipeline failed. Review Console Output and the relevant SSM command output.'
        }
        always {
            sh '''
                set +e
                echo "Docker usage before cleanup:"
                docker system df
                docker builder prune -af
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
