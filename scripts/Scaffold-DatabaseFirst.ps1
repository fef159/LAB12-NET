$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path $PSScriptRoot -Parent
$databaseName = 'Lab12VeterinariaScaffold'
$server = '(localdb)\MSSQLLocalDB'

sqlcmd -S $server -E -Q "IF DB_ID(N'$databaseName') IS NULL CREATE DATABASE [$databaseName];" -b
sqlcmd -S $server -E -d $databaseName -i (Join-Path $repoRoot 'database.sql') -b

Push-Location $repoRoot
try {
    dotnet tool restore
    dotnet tool run dotnet-ef dbcontext scaffold `
        "Server=$server;Database=$databaseName;Trusted_Connection=True;TrustServerCertificate=True" `
        Microsoft.EntityFrameworkCore.SqlServer `
        --context VeterinariaScaffoldContext `
        --context-dir Scaffolded/Data `
        --output-dir Scaffolded/Models `
        --namespace Lab12NetApi.Scaffolded.Models `
        --context-namespace Lab12NetApi.Scaffolded.Data `
        --data-annotations `
        --no-onconfiguring `
        --table dbo.Propietarios `
        --table dbo.Mascotas `
        --force
}
finally {
    Pop-Location
}

Write-Host 'Scaffold completado. Archivos generados en Scaffolded/Models y Scaffolded/Data.'
