$publicApi = 'http://localhost:11081'
$emailApi = 'http://localhost:10084'

Write-Host '############################################'
Write-Host '## Performing full end to end system test ##'
Write-Host '############################################'
Write-Host  Get access token
try {
    $token = Invoke-RestMethod -Uri "$publicApi/token" -Method Get
}
catch {
    Write-Host "Failed to get token: $_" -ForegroundColor Red
    exit 1
}
Write-Host Access token obtained successfully -ForegroundColor Green
Write-Host '############################################'
Write-Host Create Job

try {
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

    $response = Invoke-RestMethod -Uri "$publicApi/job" -Method Post -Headers $Headers -Body $json
    $jobId = $response.jobId
}
catch {
    Write-Output "Failed to create new job: $_"
    exit 1
}
Write-Host Job created obtained successfully. JobId:$jobId -ForegroundColor Green
Write-Host '############################################'
Write-Host Monitor progress

$timeout = (Get-Date).AddSeconds(60)
$lastStatus = ''

$headers = @{
    'Authorization' = "Bearer $token"
    'Content-Type' = 'application/json'
    'Accept' = 'application/json'
}

:whileWaiting while ((Get-Date) -lt $timeout) {

    try {
        $response = Invoke-RestMethod -Uri "$publicApi/job/$jobId" -Method Get -Headers $Headers
        $status = $response.status
        if ($lastStatus -ne $status) {
            Write-Host $status -NoNewline -ForegroundColor Gray
        }
        Write-Host '.' -ForegroundColor Gray -NoNewline
        switch ($status) {
            'Accepted' {
            }
            'Processing' {
            }
            'Failed' {
                Write-Host
                Write-Host "Additional Information: $($response.AdditionalInformation)" -ForegroundColor Red
                break whileWaiting
            }
            'Complete' {
                break whileWaiting
            }
        }

        $lastStatus = $status
        Start-Sleep -Milliseconds 500

    }
    catch {
        Write-Host
        Write-Host "Failed to check job status: $_" -ForegroundColor Red
        exit 1
    }
}
Write-Host
if ($status -ne 'Complete') {
    Write-Host '############################################'
    Write-Host "Failed to process job. Status: $status" -ForegroundColor Red
    exit 1
}
Write-Host '############################################'
Write-Host Processing completed successfully -ForegroundColor Green
Write-Host '############################################'

# When using AWS without LocalStack Pro, the email will not be verified
$awsNotPro = $true
try {
    $json = $(docker container inspect api.microservices-email -f 'json')
    $inspect = $json | ConvertFrom-Json
    $aws = $inspect[0].Config.Env.Contains('Microservices.CloudProvider=AWS')
    if ($aws) {

        Write-Host 'AWS detected, checking for LocalStack Pro'
        $json = $(docker container inspect aws.microservices-infrastructure -f 'json')
        $inspect = $json | ConvertFrom-Json
        $image = $inspect[0].Config.Image
        $localstackPro = $image.Contains('-pro')
        Write-Host "LocalStack Pro: $localstackPro"

        if (!$localstackPro) {
            $awsNotPro = $false
        }
    }
}
catch {
}

if ($awsNotPro) {
    Write-Host Check Email
    $timeout = (Get-Date).AddSeconds(10)
    $foundEmail = $null
    try {
        :whileWaiting while ((Get-Date) -lt $timeout) {
            $emails = Invoke-RestMethod -Uri "$emailApi/messages" -Method Get
            foreach ($email in $emails) {
                if ($email.subject -eq "Processing complete. Job $jobId") {
                    $foundEmail = $email
                    break whileWaiting
                }
            }
            Start-Sleep -Milliseconds 500
        }
    }
    catch {
        Write-Host "Failed to check email: $_" -ForegroundColor Red
        exit 1
    }
    if ($null -ne $foundEmail) {
        Write-Host "Email from $($foundEmail.sender) received successfully" -ForegroundColor Green
        Write-Host "View may the email here: $emailApi/messages/$($foundEmail.id).html"
    }
    else {
        Write-Host "Did not get email: $_" -ForegroundColor Red
        exit 1
    }
}
else {
    Write-Host 'Email will not be verified when using AWS without LocalStack Pro' -ForegroundColor Yellow
}

Write-Host '############################################'
Write-Host Test completed successfully -ForegroundColor Green
