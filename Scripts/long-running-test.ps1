$timeout = (Get-Date).AddMinutes(120)
try {
    Set-Location $PSScriptRoot
    while ((Get-Date) -lt $timeout) {

        $randomSleepSeconds = Get-Random -Minimum 0 -Maximum 5
        Start-Sleep -Seconds $randomSleepSeconds

        ./end-to-end-test.ps1

        $randomNumber = Get-Random -Minimum 1 -Maximum 2000
        if ($randomNumber -le 10) {
            $emails = Invoke-RestMethod -Uri http://localhost:17080/search/times/1672531200/1704067199 -Method Get
        }
        elseif ($randomNumber -le 16) {
            $emails = Invoke-RestMethod -Uri http://localhost:17080/search/recipient/microservices.notifications@example.com?pageSize=2 -Method Get
        }
        elseif ($randomNumber -le 100) {
            try {
                & 'C:\Program Files\Redis\redis-cli.exe' -h localhost -p 13379 flushall
            }
            catch { }
        }
    }
    Write-Host Complete -ForegroundColor Green
}
catch {
    Write-Output "Failed: $_"
    exit 1
}
