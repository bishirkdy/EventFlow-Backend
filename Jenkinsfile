pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
                echo 'GitHub source code checked out successfully.'
            }
        }

        stage('Verify Source') {
            steps {
                sh 'pwd'
                sh 'git log -1 --oneline'
            }
        }
    }
}
