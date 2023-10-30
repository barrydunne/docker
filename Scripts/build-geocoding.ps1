Set-Location $PSScriptRoot/../Geocoding/DockerCompose
docker compose -p microservices-geocoding up -d --build
Set-Location $PSScriptRoot