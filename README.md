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
| POST | `/api/mascotas` | 201, 400 |
| PUT | `/api/mascotas/{id}` | 204, 400, 404 |
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

## Pruebas públicas

El script `tests/Test-Lab12Api.ps1` ejecuta 17 pruebas contra la API publicada y limpia los registros creados durante la ejecución. Los resultados verificados están en `docs/public-test-results.json` y la matriz visual en `docs/captura-pruebas-publicas.png`.

Resultado final: **17 pruebas aprobadas de 17, sin fallos**.

