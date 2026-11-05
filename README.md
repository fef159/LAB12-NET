# InventarioApi — solución Web API REST Semana 12

API de inventario para administrar categorías y productos. Resuelve la necesidad de consultar existencias, precios y clasificación de productos sin duplicar códigos de inventario.

## Modelo y reglas de negocio

- `Categoria 1:N Producto`; `Producto.CategoriaId` es obligatorio y usa eliminación restringida.
- No se permiten nombres de categoría repetidos (`409 Conflict`).
- No se permiten SKU repetidos (`409 Conflict`).
- No se puede eliminar una categoría que tenga productos (`409 Conflict`).
- Data Annotations validan campos obligatorios, longitudes, precio positivo y stock no negativo (`400 ValidationProblem`).

## Ejecución local

1. Ejecuta `database.sql` en SQL Server.
2. Ajusta `ConnectionStrings:DefaultConnection` en `appsettings.json` o define `ConnectionStrings__DefaultConnection` como variable de entorno.
3. Ejecuta `dotnet tool restore`, `dotnet restore` y `dotnet run --urls http://localhost:5000`.
4. Abre `http://localhost:5000/swagger`.
5. Ejecuta las solicitudes de `InventarioApi.http` para comprobar `200`, `201`, `204`, `400`, `404` y `409`.

## Endpoints

| Método | Ruta | Respuesta principal |
|---|---|---|
| GET | `/api/categorias` | 200 |
| GET | `/api/categorias/{id}` | 200 / 404 |
| POST | `/api/categorias` | 201 / 400 / 409 |
| PUT | `/api/categorias/{id}` | 204 / 400 / 404 / 409 |
| DELETE | `/api/categorias/{id}` | 204 / 404 / 409 |
| GET | `/api/productos` | 200 |
| GET | `/api/productos/{id}` | 200 / 404 |
| GET | `/api/categorias/{id}/productos` | 200 / 404 |
| POST | `/api/productos` | 201 / 400 / 409 |
| PUT | `/api/productos/{id}` | 204 / 400 / 404 / 409 |
| DELETE | `/api/productos/{id}` | 204 / 404 |

## Modelo generado con Scaffold (Database First)

Además del modelo usado por la API, la carpeta `Scaffolded` contiene el resultado real de aplicar ingeniería inversa a la base creada con `database.sql`. El comando reproducible es:

```powershell
dotnet tool restore
dotnet tool run dotnet-ef dbcontext scaffold `
  "Server=(localdb)\MSSQLLocalDB;Database=InventarioDb;Trusted_Connection=True;TrustServerCertificate=True;" `
  Microsoft.EntityFrameworkCore.SqlServer `
  --context ScaffoldedDbContext --context-dir Scaffolded/Data `
  --output-dir Scaffolded/Models --data-annotations --no-onconfiguring --force
```

`--no-onconfiguring` evita que la cadena de conexión quede escrita en el código generado.

## Pruebas ejecutadas localmente

| Caso | Código verificado |
|---|---:|
| GET de listado | 200 |
| POST válido | 201 |
| PUT y DELETE válidos | 204 |
| Data Annotations inválidas | 400 |
| Clave foránea inexistente | 400 |
| Recurso inexistente | 404 |
| Nombre o SKU repetido | 409 |
| Eliminar categoría con productos | 409 |
| Documento OpenAPI de Swagger | 200 |

La compilación Release terminó con **0 errores y 0 advertencias** y el análisis de paquetes no encontró vulnerabilidades conocidas.

## Investigación y elección de nube (consultada el 4 de noviembre de 2026)

**Elección recomendada: Azure App Service F1 + Azure SQL Database Free.** Se mantiene SQL Server, por lo que no hay que cambiar el proveedor de EF Core ni reescribir el script. La guía oficial de App Service permite seleccionar el nivel gratuito F1 para .NET 10. La oferta gratuita de Azure SQL incluye 100 000 segundos de vCore al mes, hasta 32 GB de datos y la opción de pausar al agotar el límite para no generar cargos.

- [Guía oficial de ASP.NET Core en Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore)
- [Oferta gratuita oficial de Azure SQL Database](https://learn.microsoft.com/azure/azure-sql/database/free-offer)

**Alternativa:** Render ofrece Web Services gratuitos y admite .NET mediante Docker, pero su PostgreSQL gratuito expira a los 30 días. Para usarlo habría que cambiar `Microsoft.EntityFrameworkCore.SqlServer` por `Npgsql.EntityFrameworkCore.PostgreSQL`, cambiar `UseSqlServer` por `UseNpgsql` y adaptar `database.sql` a PostgreSQL.

- [Limitaciones oficiales del plan gratuito de Render](https://render.com/docs/free)
- [Servicios web y Docker en Render](https://render.com/docs/web-services)

## Pasos de despliegue elegidos

1. Crea Azure SQL Database con la oferta gratuita y con pausa automática al agotar el límite.
2. Permite temporalmente la IP del equipo en el firewall del servidor y ejecuta `database.sql` contra la base.
3. Crea una Web App .NET 10 en App Service con plan F1.
4. En **Configuration**, guarda `ConnectionStrings__DefaultConnection` y `Cors__AllowedOrigin`; nunca subas la cadena real al repositorio.
5. Publica desde Visual Studio, VS Code o GitHub Actions y abre `https://<app>.azurewebsites.net/swagger`.
6. Ejecuta `InventarioApi.http` contra el dominio público y captura Swagger, Thunder Client o Postman mostrando cada endpoint y cada código.

La aplicación queda preparada, pero el enlace público y las capturas deben generarse desde las cuentas de nube del estudiante, porque requieren credenciales y aceptación de condiciones del proveedor.
