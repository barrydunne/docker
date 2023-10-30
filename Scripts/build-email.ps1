Set-Location $PSScriptRoot/../Email/DockerCompose
docker compose -p microservices-email up -d --build
Set-Location $PSScriptRoot