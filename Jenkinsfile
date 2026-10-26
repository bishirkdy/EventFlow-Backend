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
        AWS_REGION     = 'ap-southeast-2'
        AWS_ACCOUNT_ID = '730132962765'
        ECR_REGISTRY   = '730132962765.dkr.ecr.ap-southeast-2.amazonaws.com'

        // Verify this is your BACKEND EC2 instance ID.
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
                '''
            }
        }

        stage('Build and Push Images') {
            steps {
                sh '''
                    set -eu

                    aws ecr get-login-password \
                      --region "$AWS_REGION" \
                      | docker login \
                        --username AWS \
                        --password-stdin "$ECR_REGISTRY"

                    build_and_push() {
                        SERVICE="$1"
                        DOCKERFILE="$2"
                        IMAGE="$ECR_REGISTRY/eventflow-$SERVICE"

                        echo "Building eventflow-$SERVICE"

                        docker build \
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

                    COMMAND_ID=$(aws ssm send-command \
                      --region "$AWS_REGION" \
                      --instance-ids "$BACKEND_INSTANCE_ID" \
                      --document-name "AWS-RunShellScript" \
                      --comment "EventFlow deployment build $BUILD_NUMBER" \
                      --parameters "commands=[\\"set -eu; cd /opt/eventflow && export IMAGE_TAG=$BUILD_NUMBER && docker-compose --env-file /opt/eventflow/.env.prod -f /opt/eventflow/docker-compose.prod.yml pull gateway identity event registration operations && docker-compose --env-file /opt/eventflow/.env.prod -f /opt/eventflow/docker-compose.prod.yml up -d --no-build gateway identity event registration operations\\"]" \
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

                    echo "Checking backend containers and gateway health"

                    COMMAND_ID=$(aws ssm send-command \
                      --region "$AWS_REGION" \
                      --instance-ids "$BACKEND_INSTANCE_ID" \
                      --document-name "AWS-RunShellScript" \
                      --comment "Verify EventFlow build $BUILD_NUMBER" \
                      --parameters "commands=[\\"set -eu; cd /opt/eventflow && docker-compose --env-file /opt/eventflow/.env.prod -f /opt/eventflow/docker-compose.prod.yml ps && for i in 1 2 3 4 5 6; do CODE=\\\\\\$(curl -sS -o /dev/null -w '%{http_code}' --max-time 10 http://127.0.0.1:5000/api/v1/auth/profile) || CODE=000; case \\\\\\${CODE} in 200|401|403) echo Gateway responded successfully; exit 0 ;; esac; echo Gateway not ready: HTTP \\\\\\${CODE}; sleep 10; done; echo Gateway health verification failed; exit 1\\"]" \
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
            echo 'Build, ECR push, SSM deployment and gateway verification completed.'
        }

        failure {
            echo 'Pipeline failed. Check Console Output and inspect the SSM command results.'
        }

        always {
            sh '''
                set +e

                echo "Docker usage before cleanup:"
                docker system df

                echo "Removing Docker build cache..."
                docker builder prune -af

                echo "Removing unused Docker images..."
                docker image prune -af

                echo "Docker usage after cleanup:"
                docker system df

                echo "Disk usage:"
                df -h /
            '''

            echo 'EventFlow CI/CD pipeline finished.'
        }
    }
}