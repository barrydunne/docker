Set-Location $PSScriptRoot/../Imaging/DockerCompose
docker compose -p microservices-imaging up -d --build
Set-Location $PSScriptRoot