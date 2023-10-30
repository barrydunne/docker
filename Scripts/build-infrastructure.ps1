Set-Location $PSScriptRoot/../Infrastructure/DockerCompose
docker compose -p microservices-infrastructure up -d --build
Set-Location $PSScriptRoot