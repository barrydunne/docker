<#
If API keys are available for MapQuest and Bing they can be supplied in api-keys.json
For example:

{
    "MapQuest": "#######",
    "Bing": "#######"
}

#>
$apiKeysFile = Join-Path -Path $PSScriptRoot -ChildPath 'api-keys.json'
if (Test-Path $apiKeysFile -PathType Leaf) {
    $apiKeys = Get-Content -Path $apiKeysFile -Raw | ConvertFrom-Json
    $mapQuestApiKey = $apiKeys.MapQuest
    $bingApiKey = $apiKeys.Bing
}
else {
    $mapQuestApiKey = 'UNAVAILABLE-USE-DUMMY-SERVICE'
    $bingApiKey = 'UNAVAILABLE-USE-DUMMY-SERVICE'
}

Write-Host Initializing AWS
$csproj = @'
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup><OutputType>Exe</OutputType>
        <TargetFramework>net8.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
    </PropertyGroup>
    <ItemGroup>
        <PackageReference Include="AWSSDK.Extensions.NETCore.Setup" Version="3.7.301" />
        <PackageReference Include="AWSSDK.S3" Version="3.7.309" />
        <PackageReference Include="AWSSDK.SecretsManager" Version="3.7.303.9" />
        <PackageReference Include="AWSSDK.SimpleEmail" Version="3.7.300.102" />
        <PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
        <PackageReference Include="Microsoft.Extensions.Configuration.EnvironmentVariables" Version="8.0.0" />
        <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
    </ItemGroup>
</Project>
'@
$cs = @"
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "test");
Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "test");
Environment.SetEnvironmentVariable("AWS__AuthenticationRegion", "eu-west-1");
Environment.SetEnvironmentVariable("AWS__ServiceURL", "http://localhost:10566");

var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
var sp = new ServiceCollection()
    .AddDefaultAWSOptions(config.GetAWSOptions())
    .AddAWSService<IAmazonS3>()
    .AddAWSService<IAmazonSecretsManager>()
    .AddAWSService<IAmazonSimpleEmailService>()
    .BuildServiceProvider();
var amazonSecretsManager = sp.GetRequiredService<IAmazonSecretsManager>();
var amazonSimpleEmailService = sp.GetRequiredService<IAmazonSimpleEmailService>();

var amazonS3 = sp.GetRequiredService<IAmazonS3>();
(amazonS3.Config as AmazonS3Config)!.ForcePathStyle = true;
await amazonS3.PutBucketAsync(new PutBucketRequest { BucketName = "imaging", BucketRegion = new S3Region("eu-west-1") });

Console.WriteLine("Verifying email identity: microservices-notifications@example.com");
await amazonSimpleEmailService.VerifyEmailIdentityAsync(new VerifyEmailIdentityRequest() { EmailAddress = "microservices-notifications@example.com" });

Console.WriteLine("Storing secrets");
await amazonSecretsManager.CreateSecretAsync(new CreateSecretRequest { Name = "infrastructure", SecretString = "{\"rabbit.user\":\"admin\",\"rabbit.password\":\"P@ssw0rd\",\"rabbit.vhost\":\"microservices\"}" });
await amazonSecretsManager.CreateSecretAsync(new CreateSecretRequest { Name = "publicapi", SecretString = "{\"mongo.connectionstring\":\"mongodb://admin:P%40ssw0rd@mongo.microservices-publicapi:27017?retryWrites=true&directConnection=true&authSource=admin\"}" });
await amazonSecretsManager.CreateSecretAsync(new CreateSecretRequest { Name = "state", SecretString = "{\"mongo.connectionstring\":\"mongodb://admin:P%40ssw0rd@mongo.microservices-state:27017?retryWrites=true&directConnection=true&authSource=admin\"}" });
await amazonSecretsManager.CreateSecretAsync(new CreateSecretRequest { Name = "email", SecretString = "{\"mysql.connectionstring\":\"Server=mysql.microservices-email;Database=email;Uid=admin;Pwd=P@ssw0rd;\"}" });
await amazonSecretsManager.CreateSecretAsync(new CreateSecretRequest { Name = "api.keys", SecretString = "{\"geocoding.mapquest\":\"$mapQuestApiKey\",\"directions.mapquest\":\"$mapQuestApiKey\",\"imaging.bing\":\"$bingApiKey\"}" });

Console.WriteLine("AWS initialized successfully!");
"@

$tmp = Join-Path -Path $PSScriptRoot -ChildPath 'temp'
New-Item $tmp -ItemType Directory -Force -ErrorAction SilentlyContinue | Out-Null

$dir = Join-Path -Path $tmp -ChildPath 'aws.init'
New-Item -ItemType Directory -Path $dir -Force -ErrorAction SilentlyContinue | Out-Null
$path = Join-Path -Path $dir -ChildPath 'program.cs'
Set-Content -Path $path -Value $cs
$path = Join-Path -Path $dir -ChildPath 'aws.init.csproj'
Set-Content -Path $path -Value $csproj
Set-Location $dir
dotnet run

Set-Location $PSScriptRoot
Remove-Item -Path $tmp -Recurse -Force -ErrorAction SilentlyContinue | Out-Null
