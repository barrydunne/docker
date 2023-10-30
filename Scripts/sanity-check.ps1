$publicApi = 'http://localhost:11081'

try {
    $token = Invoke-RestMethod -Uri "$publicApi/token" -Method Get
    $idempotencyKey = [System.Guid]::NewGuid().ToString()
    $headers = @{
        'X-Idempotency-Key' = $idempotencyKey
        'Authorization' = "Bearer $token"
        'Content-Type' = 'application/json'
        'Accept' = 'application/json'
    }
    $json = @'
    {
        "startingAddress": "Silver Creek Court, Orlando, FL 34714, United States",
        "destinationAddress": "Chocolate Emporium, 6000 Universal Blvd, Orlando, FL 32819, United States",
        "email": "microservices.notifications@example.com"
    }
'@
    $jobId = Invoke-RestMethod -Uri "$publicApi/job" -Method Post -Headers $Headers -Body $json
}
catch {
    Write-Output "Failed to create new job: $_"
    exit 1
}
