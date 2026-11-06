# Despliegue de InventarioApi en Azure

Esta guía publica la API en Azure App Service F1 y los datos en Azure SQL Database Free. La solución conserva SQL Server y no requiere cambiar el proveedor de Entity Framework Core.

## 1 Crear la base de datos gratuita

1. En Azure Portal, abre **Azure SQL** y selecciona **Crear base de datos SQL**.
2. Crea o selecciona una suscripción, un grupo de recursos y un servidor lógico.
3. En la sección de proceso, selecciona la oferta gratuita y configura **Pausar automáticamente la base de datos hasta el próximo mes** cuando se agote el límite. Así se evitan cargos por excedente.
4. Crea la base con el nombre `InventarioDb`.
5. En **Redes** del servidor SQL, habilita el acceso desde servicios de Azure y agrega temporalmente la IP pública del equipo desde el que administrarás la base.
6. Abre el editor de consultas de la base `InventarioDb` y ejecuta todo el archivo `database.azure.sql`.

El script para Azure no contiene `CREATE DATABASE` ni `USE`, porque se ejecuta ya conectado a la base creada por el portal. Puede ejecutarse nuevamente sin duplicar tablas ni datos de ejemplo.

## 2 Crear la aplicación web

1. En Azure Portal, crea un recurso **Web App**.
2. Elige **Código**, runtime **.NET 10**, sistema operativo Windows o Linux y plan **Free F1**.
3. Conserva la URL pública que Azure muestra en el recurso. Los recursos nuevos pueden usar un hostname con sufijo único, por ejemplo `https://inventario-api-abc123.region.azurewebsites.net`.
4. En **Variables de entorno** agrega:

   | Nombre | Valor |
   |---|---|
   | `ConnectionStrings__DefaultConnection` | Cadena ADO.NET de `InventarioDb` obtenida en Azure SQL |
   | `Cors__AllowedOrigin` | Origen HTTPS concreto permitido para el cliente web |
   | `ASPNETCORE_ENVIRONMENT` | `Production` |

La cadena debe incluir `Encrypt=True` y `TrustServerCertificate=False`. No la copies en archivos del repositorio.

## 3 Conectar GitHub Actions con App Service

1. En la Web App, abre **Descargar perfil de publicación**. Si la opción está deshabilitada, habilita temporalmente la autenticación básica de publicación SCM en la configuración de la aplicación.
2. En GitHub abre **Settings > Secrets and variables > Actions**.
3. Crea el secreto `AZURE_WEBAPP_PUBLISH_PROFILE` y pega el contenido completo del perfil descargado.
4. En la pestaña **Variables**, crea `AZURE_WEBAPP_NAME` con el nombre del recurso Web App, no con su URL.
5. En **Actions**, abre **Desplegar API en Azure App Service** y selecciona **Run workflow**.

El perfil de publicación queda cifrado como secreto de GitHub y nunca se guarda en el código.

## 4 Verificar el despliegue

Cuando el flujo termine correctamente, comprueba estas direcciones usando el hostname real asignado por Azure:

- `https://<hostname>/health` debe responder `Healthy` con código 200.
- `https://<hostname>/swagger` debe mostrar Swagger UI.
- `https://<hostname>/api/categorias` debe devolver los datos de ejemplo con código 200.
- `https://<hostname>/api/productos` debe devolver los productos con código 200.

Después, cambia la variable `@host` de `InventarioApi.http` por la URL pública y ejecuta todos los casos 200, 201, 204, 400, 404 y 409. Las capturas para la entrega deben mostrar el dominio público, no `localhost`.

## 5 Diagnóstico rápido

- **Error 500.30 al iniciar:** falta `ConnectionStrings__DefaultConnection` o la cadena tiene un formato incorrecto.
- **Error de inicio de sesión SQL:** revisa usuario, contraseña y nombre del servidor lógico.
- **Timeout al consultar SQL:** revisa el firewall del servidor y la opción para permitir servicios de Azure.
- **La primera consulta tarda:** la base gratuita puede estar pausada y necesita reactivarse.
- **GitHub Actions no despliega:** confirma que `AZURE_WEBAPP_NAME` coincide exactamente con el recurso y vuelve a descargar el perfil si fue regenerado.
