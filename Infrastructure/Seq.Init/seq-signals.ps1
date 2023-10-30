$seqHost = 'http://seq'
# Use localhost for development
# $seqHost = 'http://localhost:10081'

function Get-SeqData {
    param(
        [Parameter(Mandatory)]$dataType
    )

    $url = "$seqHost/api/$dataType/?ownerId=user-admin&shared=true"

    for ($attempt = 1; $attempt -le 12; $attempt++) {
        try {
            Write-Host "Getting $dataType list from $url"
            $data = Invoke-RestMethod -Uri $url
            if ($data.Count -ge 0) {
                Write-Host "Found $($data.Count) $dataType"
                return $data
            }
        }
        catch {
            Write-Host "Failed on attempt $attempt"
            Write-Host $_.Exception.Message
            if ($attempt -lt 12) {
                Start-Sleep -Seconds 5
            }
        }
    }
    return $null
}

function New-SeqData {
    param(
        [Parameter(Mandatory)]$dataType,
        [Parameter(Mandatory)]$existing,
        [Parameter(Mandatory)]$title,
        [Parameter(Mandatory)]$json
    )

    $known = $existing | Where-Object { $_.Title -eq $title }
    if ($null -ne $known) {
        Write-Host "'$title' already exists in $dataType"

        $updateUrl = "$seqHost/$($known.Links.Self)"
        try {
            # Need to maintian original Id and links
            $obj = $json | ConvertFrom-Json
            $obj.Id = $known.Id
            $obj.Links = $known.Links
            $json = $obj | ConvertTo-Json
            Write-Host $json

            Write-Host "Updating $dataType entity for '$title'"
            $response = Invoke-RestMethod -Uri $updateUrl -Method Put -Headers $headers -Body $json
            return $response.Id
        }
        catch {
            Write-Host "Failed to create $dataType entity"
            Write-Host $_.Exception.Message
        }

        return
    }

    Write-Host "Creating $dataType entity for '$title'"
    Write-Host $json
    $url = "$seqHost/api/$dataType/"
    $headers = @{
        "Content-Type" = "application/json"
    }
    try {
        $response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $json
        return $response.Id
    }
    catch {
        Write-Host "Failed to create $dataType entity"
        Write-Host $_.Exception.Message
    }
}

function New-SeqRetentionPeriod {
    param(
        [Parameter(Mandatory)]$time
    )

    $url = "$seqHost/api/retentionpolicies/"

    $existing = Get-SeqData -dataType 'retentionpolicies'
    foreach ($policy in $existing) {
        $id = $policy.Id
        $deleteUrl = "$url$id"
        Write-Host "Deleting existing retention policy $Id"
        Invoke-RestMethod -Uri $deleteUrl -Method Delete
    }

    Write-Host "Setting retention period to $time"
    $json = @"
    {
        "RetentionTime": "$($time)",
        "RemovedSignalExpression": null,
        "Id": null,
        "Links": {
            "Create": "api/retentionpolicies/"
        }
    }
"@
    $headers = @{
        "Content-Type" = "application/json"
    }
    try {
        $response = Invoke-RestMethod -Uri $url -Method Post -Headers $headers -Body $json
        return $response.Id
    }
    catch {
        Write-Host "Failed to set retention period"
        Write-Host $_.Exception.Message
    }
}

$signalApplicationJson = @"
{
    "Title": "Show Application Column",
    "Description": "Include Application as a column",
    "Filters": [],
    "Columns": [
        {
            "Expression": "Application"
        }
    ],
    "IsProtected": false,
    "IsWatched": true,
    "Grouping": "Explicit",
    "ExplicitGroupName": "Application",
    "OwnerId": null,
    "Id": null,
    "Links": {
        "Create": "api/signals/"
    }
}
"@
$signalDebugJson = @"
{
    "Title": "Debug",
    "Description": "Filter debug level",
    "Filters": [
        {
            "Filter": "@Level in ['debug'] ci",
            "FilterNonStrict": "@Level in ['debug'] ci"
        }
    ],
    "Columns": [],
    "IsProtected": false,
    "IsWatched": false,
    "Grouping": "Explicit",
    "ExplicitGroupName": "@Level",
    "OwnerId": null,
    "Id": null,
    "Links": {
        "Create": "api/signals/"
    }
}
"@
$signalInfoJson = @"
{
    "Title": "Info",
    "Description": "Filter info level",
    "Filters": [
        {
            "Filter": "@Level in ['info','i','information'] ci",
            "FilterNonStrict": "@Level in ['info','i','information'] ci"
        }
    ],
    "Columns": [],
    "IsProtected": false,
    "IsWatched": false,
    "Grouping": "Explicit",
    "ExplicitGroupName": "@Level",
    "OwnerId": null,
    "Id": null,
    "Links": {
        "Create": "api/signals/"
    }
}
"@
$signalTraceJson = @"
{
    "Title": "Trace",
    "Description": "Filter trace level",
    "Filters": [
        {
            "Filter": "@Level in ['trace'] ci",
            "FilterNonStrict": "@Level in ['trace'] ci"
        }
    ],
    "Columns": [],
    "IsProtected": false,
    "IsWatched": false,
    "Grouping": "Explicit",
    "ExplicitGroupName": "@Level",
    "OwnerId": null,
    "Id": null,
    "Links": {
        "Create": "api/signals/"
    }
}
"@

$signals = Get-SeqData -dataType 'signals'
if ($null -ne $signals) {
    New-SeqData -dataType 'signals' -existing $signals -title 'Show Application Column' -json $signalApplicationJson
    New-SeqData -dataType 'signals' -existing $signals -title 'Debug' -json $signalDebugJson
    New-SeqData -dataType 'signals' -existing $signals -title 'Info' -json $signalInfoJson
    New-SeqData -dataType 'signals' -existing $signals -title 'Trace' -json $signalTraceJson

    $signals = Get-SeqData -dataType 'signals'
    $signalIds = $signals | ForEach-Object { "`"$($_.Id)`"" }
    $signalIdsString = $signalIds -join ', '

    $workspaceJson = @"
{
    "Title": "Personal",
    "Description": null,
    "OwnerId": "user-admin",
    "IsProtected": false,
    "DefaultSignalExpression": null,
    "Content": {
        "SignalIds": [
            $($signalIdsString)
        ],
        "QueryIds": [
            "sqlquery-2",
            "sqlquery-3",
            "sqlquery-4",
            "sqlquery-5"
        ],
        "DashboardIds": [
            "dashboard-14"
        ]
    },
    "Id": null,
    "Links": {
        "Create": "api/workspaces/"
    }
}
"@

    $workspaces = Get-SeqData -dataType 'workspaces'
    if ($null -ne $workspaces) {
        New-SeqData -dataType 'workspaces' -existing $workspaces -title 'Personal' -json $workspaceJson
    }

    New-SeqRetentionPeriod -time '2.0:0:00'

    Write-Host 'Configuration Complete'
}
