
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
        AWS_REGION = 'ap-southeast-2'
        AWS_ACCOUNT_ID = '730132962765'
        ECR_REGISTRY = '730132962765.dkr.ecr.ap-southeast-2.amazonaws.com'
        BACKEND_HOST = '3.25.150.159'
        BACKEND_USER = 'ec2-user'
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
                        echo "Clean up disk space before building."
                        exit 1
                    fi

                    docker info >/dev/null
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

        stage('Deploy to Backend EC2') {
            steps {
                sshagent(credentials: ['eventflow-backend-ssh']) {
                    sh '''
                        set -eu

                        echo "Deploying build $BUILD_NUMBER"

                        ssh \
                          -o StrictHostKeyChecking=accept-new \
                          -o ConnectTimeout=15 \
                          "$BACKEND_USER@$BACKEND_HOST" \
                          "cd /opt/eventflow && \
                           export IMAGE_TAG=$BUILD_NUMBER && \
                           docker-compose \
                             --env-file /opt/eventflow/.env.prod \
                             -f /opt/eventflow/docker-compose.prod.yml \
                             pull gateway identity event registration operations && \
                           docker-compose \
                             --env-file /opt/eventflow/.env.prod \
                             -f /opt/eventflow/docker-compose.prod.yml \
                             up -d --no-build \
                             gateway identity event registration operations"
                    '''
                }
            }
        }

        stage('Verify Deployment') {
            steps {
                sshagent(credentials: ['eventflow-backend-ssh']) {
                    sh '''
                        set -eu

                        ssh \
                          -o StrictHostKeyChecking=accept-new \
                          -o ConnectTimeout=15 \
                          "$BACKEND_USER@$BACKEND_HOST" \
                          "cd /opt/eventflow && \
                           docker-compose \
                             --env-file /opt/eventflow/.env.prod \
                             -f /opt/eventflow/docker-compose.prod.yml \
                             ps && \
                           for i in 1 2 3 4 5 6; do \
                             CODE=\\$(curl -sS -o /dev/null \
                               -w '%{http_code}' \
                               --max-time 10 \
                               http://127.0.0.1:5000/api/v1/auth/profile) || CODE=000; \
                             case \\$CODE in \
                               200|401|403) \
                                 echo 'Gateway responded successfully'; exit 0 ;; \
                             esac; \
                             echo \\\"Gateway not ready yet: HTTP \\$CODE\\\"; \
                             sleep 10; \
                           done; \
                           echo 'ERROR: Gateway health verification failed'; exit 1"
                    '''
                }
            }
        }
    }

    post {
        success {
            echo 'Build, ECR push, deployment and gateway verification completed.'
        }

        failure {
            echo 'Pipeline failed. Check Console Output and verify backend service health.'
        }

        always {
            sh '''
                set +e

                echo "Docker usage before cleanup:"
                docker system df

                echo "Removing Docker build cache older than 3 days..."
                docker builder prune -af --filter "until=72h"

                echo "Removing unused images older than 7 days..."
                docker image prune -af --filter "until=168h"

                echo "Docker usage after cleanup:"
                docker system df

                echo "Disk usage:"
                df -h /
            '''
            echo 'EventFlow CI/CD pipeline finished.'
        }
    }
}
