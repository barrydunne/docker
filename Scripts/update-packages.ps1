$baseDir = Split-Path -Path $PSScriptRoot -Parent

$csprojFiles = Get-ChildItem -Path $baseDir -Filter *.csproj -Recurse

$packages = @{}
foreach ($csprojFile in $csprojFiles) {
    $content = Get-Content -Path $csprojFile.FullName
    $content | Select-String -Pattern '<PackageReference Include="(.*)" Version="(.*)"' | ForEach-Object {
        $packageName = $_.Matches.Groups[1].Value
        if (!$packageName.StartsWith('Microservices.')) {
            $packageVersion = $_.Matches.Groups[2].Value
            if ($packages.ContainsKey($packageName) -and $packages[$packageName] -ne $packageVersion) {
                Write-Host "Package $packageName has different versions: $($packages[$packageName]) and $packageVersion"
                exit 1
            }
            $packages[$packageName] = $packageVersion
        }
    }
}

# Check for updates to all packages
$updates = @{}
$packages.GetEnumerator() | Sort-Object -Property Key | ForEach-Object {
    $package = $_.Key
    $version = $_.Value
    $latestVersion = ''
    $uri = "https://api.nuget.org/v3-flatcontainer/$($_.Key)/index.json".ToLower()
    Write-Host "Checking $uri"

    if ($version.Contains('-')) {
        $latestVersion = Invoke-WebRequest -Uri $uri -UseBasicParsing | ConvertFrom-Json | Select-Object -ExpandProperty versions | Select-Object -Last 1
    }
    else {
        # Select the latest version that is not a pre-release
        $latestVersion = Invoke-WebRequest -Uri $uri -UseBasicParsing | ConvertFrom-Json | Select-Object -ExpandProperty versions | Where-Object { $_ -notlike '*-*' } | Select-Object -Last 1
    }

    if ($latestVersion -eq '') {
        Write-Host "Did not found $package from $uri"
        continue
    }

    if ($latestVersion -ne $version) {
        Write-Host "Found updated $package from $version to $latestVersion"
        $updates[$package] = $latestVersion
    }
}

# Update all packages
foreach ($csprojFile in $csprojFiles) {
    Write-Host "Checking $csprojFile"
    $content = Get-Content -Path $csprojFile.FullName -Raw
    $modified = $false
    $updates.GetEnumerator() | ForEach-Object {
        $package = $_.Key
        $fromVersion = $packages[$package]
        $toVersion = $updates[$package]
        $from = "<PackageReference Include=""$package"" Version=""$fromVersion"""
        $to   = "<PackageReference Include=""$package"" Version=""$toVersion"""
        if ($content.Contains($from)) {
            $content = $content.Replace($from, $to)
            $modified = $true
        }
    }
    if ($modified) {
        Write-Host "Updating $csprojFile"
        # Write content to file maintaining the original encoding
        [System.IO.File]::WriteAllText($csprojFile.FullName, $content, [System.Text.Encoding]::UTF8)
    }
}