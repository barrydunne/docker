Set-Location $PSScriptRoot/../Weather/DockerCompose
docker compose -p microservices-weather up -d --build
Set-Location $PSScriptRoot