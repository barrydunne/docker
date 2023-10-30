Set-Location $PSScriptRoot/../State/DockerCompose
docker compose -p microservices-state up -d --build
Set-Location $PSScriptRoot