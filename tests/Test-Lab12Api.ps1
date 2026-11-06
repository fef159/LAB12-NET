$ErrorActionPreference = 'Stop'
$baseUrl = 'https://lab12-net-app-9k2m-dfafgxhbbkbthtbt.westus3-01.azurewebsites.net'
$results = [System.Collections.Generic.List[object]]::new()
$ownerId = $null
$petId = $null

function Invoke-ApiTest {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Path,
        [int]$Expected,
        [object]$Body = $null,
        [scriptblock]$ExtraCheck = $null
    )
    $parameters = @{
        Uri = "$baseUrl$Path"
        Method = $Method
        SkipHttpErrorCheck = $true
        TimeoutSec = 60
    }
    if ($null -ne $Body) {
        $parameters.ContentType = 'application/json'
        $parameters.Body = ($Body | ConvertTo-Json -Depth 8 -Compress)
    }
    $response = Invoke-WebRequest @parameters
    $contentText = if ($response.Content -is [byte[]]) {
        [System.Text.Encoding]::UTF8.GetString($response.Content)
    } else {
        [string]$response.Content
    }
    $extraOk = if ($null -eq $ExtraCheck) { $true } else { & $ExtraCheck $response }
    $passed = ($response.StatusCode -eq $Expected) -and $extraOk
    $results.Add([pscustomobject]@{
        Prueba = $Name
        Metodo = $Method
        Ruta = $Path
        Esperado = $Expected
        Obtenido = [int]$response.StatusCode
        Resultado = if ($passed) { 'APROBADO' } else { 'FALLÓ' }
        Detalle = if ($contentText.Length -gt 220) { $contentText.Substring(0, 220) + '…' } else { $contentText }
    })
    if (-not $passed) { throw "Falló: $Name. HTTP $($response.StatusCode), esperado $Expected." }
    return $response
}

try {
    Invoke-ApiTest 'Swagger publicado' GET '/swagger/index.html' 200 | Out-Null
    Invoke-ApiTest 'Estado usa Azure SQL' GET '/api/estado' 200 -ExtraCheck { param($r) $r.Content -match 'SqlServer' } | Out-Null
    Invoke-ApiTest 'Lista de propietarios' GET '/api/propietarios' 200 | Out-Null
    Invoke-ApiTest 'Propietario inexistente' GET '/api/propietarios/999999' 404 | Out-Null
    Invoke-ApiTest 'Validación automática del modelo' POST '/api/propietarios' 400 -Body @{ nombre = 'A'; email = 'correo-invalido'; telefono = '' } | Out-Null

    $email = "codex.lab12.$([DateTimeOffset]::UtcNow.ToUnixTimeSeconds())@example.com"
    $createdOwner = Invoke-ApiTest 'Crear propietario' POST '/api/propietarios' 201 -Body @{
        id = 0; nombre = 'Prueba Despliegue'; email = $email; telefono = '+51 900 000 001'
    }
    $owner = $createdOwner.Content | ConvertFrom-Json
    $ownerId = [int]$owner.id

    Invoke-ApiTest 'PUT con id inconsistente' PUT "/api/propietarios/$ownerId" 400 -Body @{
        id = $ownerId + 1; nombre = 'Prueba Despliegue'; email = $email; telefono = '+51 900 000 001'
    } | Out-Null
    Invoke-ApiTest 'Actualizar propietario' PUT "/api/propietarios/$ownerId" 204 -Body @{
        id = $ownerId; nombre = 'Prueba Actualizada'; email = $email; telefono = '+51 900 000 002'
    } | Out-Null
    Invoke-ApiTest 'Correo duplicado' POST '/api/propietarios' 409 -Body @{
        id = 0; nombre = 'Propietario Duplicado'; email = $email; telefono = '+51 900 000 003'
    } | Out-Null
    Invoke-ApiTest 'Filtro LINQ por especie' GET '/api/mascotas?especie=Perro' 200 | Out-Null
    Invoke-ApiTest 'Ruta anidada sin mascotas' GET "/api/propietarios/$ownerId/mascotas" 200 | Out-Null
    Invoke-ApiTest 'Validación de clave foránea' POST '/api/mascotas' 400 -Body @{
        id = 0; nombre = 'Sin dueño'; especie = 'Perro'; edad = 2; propietarioId = 999999
    } | Out-Null

    $createdPet = Invoke-ApiTest 'Crear mascota' POST '/api/mascotas' 201 -Body @{
        id = 0; nombre = 'Nube'; especie = 'Gato'; edad = 3; propietarioId = $ownerId
    }
    $pet = $createdPet.Content | ConvertFrom-Json
    $petId = [int]$pet.id

    Invoke-ApiTest 'Ruta anidada con datos' GET "/api/propietarios/$ownerId/mascotas" 200 -ExtraCheck { param($r) $r.Content -match 'Nube' } | Out-Null
    Invoke-ApiTest 'Integridad referencial al eliminar' DELETE "/api/propietarios/$ownerId" 409 | Out-Null
    Invoke-ApiTest 'Eliminar mascota' DELETE "/api/mascotas/$petId" 204 | Out-Null
    $petId = $null
    Invoke-ApiTest 'Eliminar propietario' DELETE "/api/propietarios/$ownerId" 204 | Out-Null
    $ownerId = $null
}
finally {
    if ($null -ne $petId) { Invoke-WebRequest -Uri "$baseUrl/api/mascotas/$petId" -Method DELETE -SkipHttpErrorCheck -TimeoutSec 30 | Out-Null }
    if ($null -ne $ownerId) { Invoke-WebRequest -Uri "$baseUrl/api/propietarios/$ownerId" -Method DELETE -SkipHttpErrorCheck -TimeoutSec 30 | Out-Null }
}

$summary = [pscustomobject]@{
    Url = $baseUrl
    EjecutadoUtc = [DateTimeOffset]::UtcNow.ToString('u')
    Total = $results.Count
    Aprobadas = @($results | Where-Object Resultado -eq 'APROBADO').Count
    Fallidas = @($results | Where-Object Resultado -ne 'APROBADO').Count
    Pruebas = $results
}
$repoRoot = Split-Path $PSScriptRoot -Parent
$jsonPath = Join-Path $repoRoot 'docs/public-test-results.json'
$summary | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $jsonPath -Encoding UTF8
$summary | ConvertTo-Json -Depth 8

