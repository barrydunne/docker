Set-Location $PSScriptRoot/../PublicApi/DockerCompose
docker compose -p microservices-publicapi up -d --build
Set-Location $PSScriptRoot