# LAB12-NET - API Veterinaria

API REST desarrollada con ASP.NET Core .NET 10 y Entity Framework Core para administrar propietarios y mascotas. La solución demuestra una relación uno a muchos, validaciones con Data Annotations, consultas LINQ, reglas de negocio y respuestas HTTP 200, 201, 204, 400, 404 y 409.

## API publicada

- Swagger: https://lab12-net-app-9k2m-dfafgxhbbkbthtbt.westus3-01.azurewebsites.net/swagger/index.html
- Estado: https://lab12-net-app-9k2m-dfafgxhbbkbthtbt.westus3-01.azurewebsites.net/api/estado
- Hosting: Azure App Service F1, Windows, .NET 10, región West US 3
- Base de datos: Azure SQL Database Free, región Sweden Central

## Modelo de datos

- `Propietario`: Id, Nombre, Email y Teléfono.
- `Mascota`: Id, Nombre, Especie, Edad y PropietarioId.
- Relación: un propietario puede tener muchas mascotas y cada mascota pertenece a un propietario.

## Reglas de negocio

- El correo del propietario es único. Un duplicado devuelve `409 Conflict`.
- No se puede eliminar un propietario que tenga mascotas. La operación devuelve `409 Conflict`.
- No se puede crear o actualizar una mascota con un propietario inexistente. La operación devuelve `400 Bad Request`.
- Los identificadores de la ruta y del cuerpo deben coincidir en las operaciones PUT.
- Las entidades usan Data Annotations para campos obligatorios, longitudes, correo válido y rango de edad.

## Endpoints

| Método | Ruta | Respuestas principales |
|---|---|---|
| GET | `/api/estado` | 200 |
| GET | `/api/propietarios` | 200 |
| GET | `/api/propietarios/{id}` | 200, 404 |
| GET | `/api/propietarios/{id}/mascotas` | 200, 404 |
| POST | `/api/propietarios` | 201, 400, 409 |
| PUT | `/api/propietarios/{id}` | 204, 400, 404, 409 |
| DELETE | `/api/propietarios/{id}` | 204, 404, 409 |
| GET | `/api/mascotas?especie={valor}` | 200 |
| GET | `/api/mascotas/{id}` | 200, 404 |
| POST | `/api/mascotas` | 201, 400, 409 |
| PUT | `/api/mascotas/{id}` | 204, 400, 404, 409 |
| DELETE | `/api/mascotas/{id}` | 204, 404 |

## Seguridad y configuración

La cadena productiva no está almacenada en el repositorio. Azure App Service recibe `ConnectionStrings__DefaultConnection` como configuración del servicio y se autentica en Azure SQL mediante una identidad administrada del sistema. La identidad tiene únicamente los roles `db_datareader` y `db_datawriter`.

`appsettings.json` contiene solo configuración no secreta. Si no existe una cadena de conexión, la aplicación usa una base en memoria para desarrollo local.

## Ejecución local

```powershell
dotnet restore
dotnet run
```

Después, abre la dirección de Swagger indicada por la consola. Para utilizar SQL Server local, define `ConnectionStrings__DefaultConnection` mediante una variable de entorno antes de iniciar la aplicación.

## Script SQL

`database.sql` crea las tablas `Propietarios` y `Mascotas`, sus restricciones y datos de ejemplo. El script elimina las tablas existentes antes de recrearlas, por lo que debe ejecutarse únicamente en una base de datos de laboratorio.

## Database First con Scaffold

El modelo Database First se generó realmente con `dotnet ef dbcontext scaffold` a partir de una base SQL Server creada con `database.sql`. Los archivos generados están en `Scaffolded/Models` y `Scaffolded/Data`; se utilizó `--no-onconfiguring`, por lo que no contienen ninguna cadena de conexión. El manifiesto `.config/dotnet-tools.json` fija `dotnet-ef` 10.0.14.

Para reproducir todo el procedimiento en Windows con SQL Server LocalDB:

```powershell
.\scripts\Scaffold-DatabaseFirst.ps1
```

El código operativo de `Models` y `Data/VeterinariaContext.cs` conserva los ajustes posteriores requeridos por la práctica: Data Annotations adicionales, navegación hija nullable y `JsonIgnore` para evitar ciclos. La explicación completa y el comando expandido están en `docs/SCAFFOLD-DATABASE-FIRST.md`.

## Elección de servicios gratuitos

Se eligió Azure App Service F1 porque ejecuta ASP.NET Core .NET 10 directamente, integra despliegue ZIP, Swagger, variables de entorno e identidad administrada. Frente a alternativas como Render o Railway, evita adaptar el proyecto a un contenedor o depender de créditos temporales y mantiene la aplicación dentro del mismo proveedor que la base de datos.

Se eligió Azure SQL Database Free porque conserva SQL Server y el proveedor `Microsoft.EntityFrameworkCore.SqlServer` utilizado durante el desarrollo. Frente a PostgreSQL gratuito en Neon o Supabase, no requiere cambiar el proveedor de EF Core ni adaptar `database.sql`. Además, la identidad administrada elimina contraseñas entre App Service y Azure SQL.

## Pruebas públicas

El script `tests/Test-Lab12Api.ps1` ejecuta 24 pruebas contra la API publicada, cubre todos los endpoints de Propietarios y Mascotas y limpia los registros creados durante la ejecución. Los resultados verificados están en `docs/public-test-results.json` y las matrices visuales en `docs/captura-pruebas-publicas-1.png` y `docs/captura-pruebas-publicas-2.png`.

Resultado final: **24 pruebas aprobadas de 24, sin fallos**.

