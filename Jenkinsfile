
pipeline {
    agent any

    options {
        disableConcurrentBuilds()
        timeout(time: 45, unit: 'MINUTES')
    }

    environment {
        AWS_REGION = 'ap-southeast-2'
        AWS_ACCOUNT_ID = '730132962765'
        ECR_REGISTRY = '730132962765.dkr.ecr.ap-southeast-2.amazonaws.com'
        BACKEND_HOST = '54.252.176.37'
        BACKEND_USER = 'ec2-user'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
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

                        echo "Deploying build $BUILD_NUMBER to backend EC2"

                        ssh \
                          -o StrictHostKeyChecking=accept-new \
                          -o ConnectTimeout=15 \
                          "$BACKEND_USER@$BACKEND_HOST" \
                          "cd /opt/eventflow && \
                           export IMAGE_TAG=$BUILD_NUMBER && \
                           docker compose \
                             --env-file /opt/eventflow/.env.prod \
                             -f /opt/eventflow/docker-compose.prod.yml \
                             pull gateway identity event registration operations && \
                           docker compose \
                             --env-file /opt/eventflow/.env.prod \
                             -f /opt/eventflow/docker-compose.prod.yml \
                             up -d --no-build \
                             gateway identity event registration operations"
                    '''
                }
            }
        }
    }

    post {
        success {
            echo 'All five backend images pushed to ECR and deployment completed.'
        }

        failure {
            echo 'Pipeline failed. Check the Console Output for the failed stage.'
        }

        always {
            echo 'EventFlow backend CI/CD pipeline finished.'
        }
    }
}
