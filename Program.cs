using Lab12NetApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Veterinaria LAB12 API",
        Version = "v1",
        Description = "API REST para propietarios y mascotas, publicada en Azure App Service."
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<VeterinariaContext>(options =>
        options.UseInMemoryDatabase("VeterinariaLab12"));
}
else
{
    builder.Services.AddDbContext<VeterinariaContext>(options =>
        options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));
}

var allowedOrigin = builder.Configuration["AllowedOrigin"]
    ?? "https://lab12-net-app-9k2m-dfafgxhbbkbthtbt.westus3-01.azurewebsites.net";
builder.Services.AddCors(options => options.AddPolicy("OrigenConcreto", policy =>
    policy.WithOrigins(allowedOrigin).AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Veterinaria LAB12 API v1");
    options.RoutePrefix = "swagger";
});

app.UseCors("OrigenConcreto");
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/api/estado", (VeterinariaContext db) => Results.Ok(new
{
    servicio = "Veterinaria LAB12 API",
    estado = "Activo",
    proveedorDatos = db.Database.ProviderName,
    utc = DateTime.UtcNow
}));

await InicializadorDatos.InicializarAsync(app.Services);

app.Run();
