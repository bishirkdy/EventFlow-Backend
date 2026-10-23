
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

                    aws ecr get-login-password --region "$AWS_REGION" \
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
    }

    post {
        success {
            echo 'All backend images built and pushed to ECR.'
        }
        failure {
            echo 'Pipeline failed. Check the stage logs.'
        }
    }
}
