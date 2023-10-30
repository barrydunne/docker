Set-Location $PSScriptRoot/../Directions/DockerCompose
docker compose -p microservices-directions up -d --build
Set-Location $PSScriptRoot