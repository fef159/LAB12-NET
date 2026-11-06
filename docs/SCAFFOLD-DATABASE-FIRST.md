# Evidencia de Database First con EF Core Scaffold

El 6 de noviembre de 2026 se creó la base local exclusiva `Lab12VeterinariaScaffold` en SQL Server LocalDB y se ejecutó `database.sql`. La comprobación de `sys.tables` devolvió las tablas `Propietarios` y `Mascotas`, con cuatro y cinco columnas respectivamente.

Después se ejecutó EF Core 10.0.14 con el proveedor `Microsoft.EntityFrameworkCore.SqlServer`:

```powershell
dotnet tool run dotnet-ef dbcontext scaffold `
  "Server=(localdb)\MSSQLLocalDB;Database=Lab12VeterinariaScaffold;Trusted_Connection=True;TrustServerCertificate=True" `
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
  --force --verbose
```

La salida detectó las dos tablas, sus claves primarias, la restricción única `UQ_Propietarios_Email` y la clave foránea `FK_Mascotas_Propietarios`. Se generaron los siguientes archivos:

- `Scaffolded/Models/Propietario.cs`
- `Scaffolded/Models/Mascota.cs`
- `Scaffolded/Data/VeterinariaScaffoldContext.cs`

El parámetro `--no-onconfiguring` evita guardar la cadena de conexión en el código generado. El script `scripts/Scaffold-DatabaseFirst.ps1` permite repetir el proceso completo. Después del Scaffold, el modelo operativo incorporó las validaciones de la práctica, la navegación hija nullable y `JsonIgnore` para la serialización JSON.
