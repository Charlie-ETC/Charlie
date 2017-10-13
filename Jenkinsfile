pipeline {
    agent any

    stages {
        stage('Clean') {
            steps {
                dir('UWP') {
                    deleteDir()
                }
            }
        }

        stage('Build Unity') {
            steps {
                withCredentials([file(credentialsId: 'charlieConfiguration', variable: 'CHARLIE_CONFIGURATION')]) {
                    powershell '''$projectPath = (Resolve-Path .\\).path
                        $logFile = Join-Path $projectPath build.log

                        $charlieConfigurationFile = Join-Path $projectPath Assets\\Resources\\Config\\local.asset
                        Copy-Item -Force $env:CHARLIE_CONFIGURATION $charlieConfigurationFile

                        New-Item -ItemType file -Force $logFile
                        $process = Start-Process \'C:\\Program Files\\Unity\\Editor\\Unity.exe\' -ArgumentList "-quit -batchmode -username yongjiew+charlie@andrew.cmu.edu -password Charliedemo123 -projectPath `"$projectPath`" -logFile `"$logFile`" -executeMethod Builder.Build" -PassThru
                        Get-Content $logFile -Wait -Tail 0 | %{ Write-Host $_; Write-Output $_ } | Select-String "Exiting batchmode" | %{ break }

                        $process.WaitForExit()
                        if ($process.ExitCode -ne 0) {
                            Exit $process.ExitCode
                        }
                        '''
                }
            }
        }

        stage('Build UWP') {
            steps {
                powershell '''$projectPath = (Resolve-Path .\\).path
                    $uwpProjectPath = Join-Path $projectPath UWP
                    $uwpSolutionPath = Join-Path $uwpProjectPath Charlie.sln

                    cd $uwpProjectPath
                    nuget restore .\\Charlie.sln
                    Import-Module Invoke-MsBuild
                    $msBuildProcess = Invoke-MsBuild -Path $uwpSolutionPath -MsBuildParameters "/property:Configuration=Debug;Platform=x86" -ShowBuildOutputInCurrentWindow -PassThru
                    $msBuildProcess.WaitForExit()
                    '''
                archiveArtifacts artifacts: 'UWP/Charlie/AppPackages/**', onlyIfSuccessful: true
            }
        }
    }
}