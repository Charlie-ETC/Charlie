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
                        $process = Start-Process "C:\\Program Files\\Unity\\Editor\\Unity.exe" -ArgumentList "-quit -batchmode -username yongjiew+charlie@andrew.cmu.edu -password Charliedemo123 -projectPath `"$projectPath`" -logFile `"$logFile`" -buildTarget wsaplayer -executeMethod Builder.Build" -PassThru

                        while (-not (Test-Path $logFile)) {
                            Start-Sleep -Seconds 1
                        }
                        Get-Content $logFile -Wait -Tail 0 | %{ Write-Host $_; Write-Output $_ } | Select-String "\\*\\*\\* Completed" | %{ break }

                        $process.WaitForExit()
                        if ($process.ExitCode -ne 0) {
                            Exit $process.ExitCode
                        }
                        '''
                }
            }
        }

        stage('Nuget Restore') {
            steps {
                powershell '''
                    $projectPath = (Resolve-Path .\\).path
                    $uwpProjectPath = Join-Path $projectPath UWP
                    $uwpSolutionPath = Join-Path $uwpProjectPath Charlie.sln

                    cd $uwpProjectPath
                    nuget restore .\\Charlie.sln
                '''
            }
        }

        stage('Build UWP') {
            parallel {
                stage('Debug') {
                    steps {
                        powershell '''$projectPath = (Resolve-Path .\\).path
                            $uwpProjectPath = Join-Path $projectPath UWP
                            $uwpSolutionPath = Join-Path $uwpProjectPath Charlie.sln

                            cd $uwpProjectPath
                            $vsPath = & "C:\\Program Files (x86)\\Microsoft Visual Studio\\Installer\\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
                            $msBuildPath = Join-Path $vsPath "MSBuild\\15.0\\Bin\\MSBuild.exe"
                            & $msBuildPath "/property:Configuration=Debug;Platform=x86"
                        '''
                    }
                }

                stage('Release') {
                    steps {
                        powershell '''$projectPath = (Resolve-Path .\\).path
                            $uwpProjectPath = Join-Path $projectPath UWP
                            $uwpSolutionPath = Join-Path $uwpProjectPath Charlie.sln

                            cd $uwpProjectPath
                            $vsPath = & "C:\\Program Files (x86)\\Microsoft Visual Studio\\Installer\\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
                            $msBuildPath = Join-Path $vsPath "MSBuild\\15.0\\Bin\\MSBuild.exe"
                            & $msBuildPath "/property:Configuration=Release;Platform=x86"
                        '''
                    }
                }
            }
        }

        stage('Archive Artifacts') {
            steps {
                archiveArtifacts artifacts: 'UWP/Charlie/AppPackages/**', onlyIfSuccessful: true
            }
        }
    }
}