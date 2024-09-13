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


Write-Host Creating api.keys/geocoding.mapquest secret
Invoke-RestMethod -Uri http://localhost:10083/secrets/vaults/api.keys/geocoding.mapquest -Method Post -Body $mapQuestApiKey -ContentType 'text/plain'


Write-Host Creating api.keys/directions.mapquest secret
Invoke-RestMethod -Uri http://localhost:10083/secrets/vaults/api.keys/directions.mapquest -Method Post -Body $mapQuestApiKey -ContentType 'text/plain'


Write-Host Creating api.keys/imaging.bing secret
Invoke-RestMethod -Uri http://localhost:10083/secrets/vaults/api.keys/imaging.bing -Method Post -Body $bingApiKey -ContentType 'text/plain'


Write-Host Creating integration.tests user in RabbitMQ
Invoke-RestMethod -Uri 'http://localhost:10672/api/users/integration.tests' -Method Put -Body '{"username":"integration.tests","password":"password","tags":"integration.tests administrator"}' -Headers @{ Authorization = 'Basic YWRtaW46UEBzc3cwcmQ=' } -ContentType 'application/json'


# Mongo DB user creation
$connectionString = 'Server=localhost;Port=17306;User Id=root;Password=P@ssw0rd;'
$csproj = @'
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup><OutputType>Exe</OutputType>
        <TargetFramework>net8.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
    </PropertyGroup>
    <ItemGroup>
        <PackageReference Include="MongoDB.Driver" Version="2.28.0" />
        <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.1"/>
    </ItemGroup>
</Project>
'@
$cs = @"
using MongoDB.Bson;
using MongoDB.Driver;
var end = DateTime.UtcNow.AddSeconds(60);
while (DateTime.UtcNow < end) {
    try
    {
        var client = new MongoClient($"mongodb://admin:P%40ssw0rd@localhost:{args[0]}");
        var database = client.GetDatabase("admin");
        var createUserCommand = new BsonDocumentCommand<BsonDocument>(new BsonDocument
        {
            { "createUser", "integration.tests" },
            { "pwd", "password" },
            { "roles", new BsonArray { "userAdminAnyDatabase", "dbAdminAnyDatabase", "readWriteAnyDatabase" } }
        });
        var result = database.RunCommand(createUserCommand);
        if (result["ok"] == 1.0)
        {
            Console.WriteLine("User created successfully!");
            break;
        }
        Console.WriteLine($"Failed, trying again in 2 seconds: {result}");
    }
    catch (Exception ex)
    {
        if (ex.Message.Contains("already exists"))
            break;
        Console.WriteLine($"Failed, trying again in 2 seconds: {ex.Message}");
    }
    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
}
"@
$tmp = Join-Path -Path $PSScriptRoot -ChildPath 'temp'
New-Item $tmp -ItemType Directory -Force -ErrorAction SilentlyContinue | Out-Null

$dir = Join-Path -Path $tmp -ChildPath 'mongouser'
New-Item -ItemType Directory -Path $dir -Force -ErrorAction SilentlyContinue | Out-Null
$path = Join-Path -Path $dir -ChildPath 'program.cs'
Set-Content -Path $path -Value $cs
$path = Join-Path -Path $dir -ChildPath 'mongouser.csproj'
Set-Content -Path $path -Value $csproj
Set-Location $dir

Write-Host Creating integration.tests user in PublicAPI mongodb
dotnet run --project ./mongouser.csproj 11017

Write-Host Creating integration.tests user in State mongodb
dotnet run --project ./mongouser.csproj 12017

Set-Location $PSScriptRoot


Write-Host Creating integration.tests user in Email MySQL
$connectionString = 'Server=localhost;Port=17306;User Id=root;Password=P@ssw0rd;'
$csproj = @'
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup><OutputType>Exe</OutputType>
        <TargetFramework>net8.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
    </PropertyGroup>
    <ItemGroup>
        <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0.2" />
        <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.1"/>
    </ItemGroup>
</Project>
'@
$cs = @"
var end = DateTime.UtcNow.AddSeconds(60);
MySqlConnector.MySqlConnection con;
while (DateTime.UtcNow < end) {
    try {
        con = new(`"$connectionString`");
        con.Open();
        if (UserExists("integration.tests"))
        {
            Console.WriteLine("User already exists");
            break;
        }
        Exec("CREATE USER 'integration.tests'@'%' IDENTIFIED BY 'password';");
        Exec("GRANT ALL PRIVILEGES ON *.* TO 'integration.tests'@'%' WITH GRANT OPTION;");
        Exec("FLUSH PRIVILEGES;");
        con.Close();
        Console.WriteLine("Complete");
        break;
    } catch (Exception ex) {
        Console.WriteLine($"Failed, trying again in 2 seconds: {ex.Message}");
        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2));
    }
}
bool UserExists(string name)
{
    using var c = con.CreateCommand();c.CommandText = "SELECT COUNT(1) FROM mysql.user WHERE User = 'integration.tests'";var result = c.ExecuteScalar();
    return ((result is not null) && Convert.ToInt32(result) > 0);
}
void Exec(string sql){Console.WriteLine(sql);using var c = con.CreateCommand();c.CommandText = sql;c.ExecuteNonQuery();}
"@
$dir = Join-Path -Path $tmp -ChildPath 'mysqluser'
New-Item -ItemType Directory -Path $dir -Force -ErrorAction SilentlyContinue | Out-Null
$path = Join-Path -Path $dir -ChildPath 'program.cs'
Set-Content -Path $path -Value $cs
$path = Join-Path -Path $dir -ChildPath 'mysqluser.csproj'
Set-Content -Path $path -Value $csproj
Set-Location $dir
dotnet run
Set-Location $PSScriptRoot

Remove-Item -Path $tmp -Recurse -Force -ErrorAction SilentlyContinue | Out-Null
