using Lab12NetApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab12NetApi.Data;

public static class InicializadorDatos
{
    public static async Task InicializarAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VeterinariaContext>();
        await db.Database.EnsureCreatedAsync();

        if (await db.Propietarios.AnyAsync()) return;

        var ana = new Propietario
        {
            Nombre = "Ana Torres",
            Email = "ana.torres@example.com",
            Telefono = "+51 987 111 222"
        };
        var luis = new Propietario
        {
            Nombre = "Luis Mendoza",
            Email = "luis.mendoza@example.com",
            Telefono = "+51 986 333 444"
        };

        db.Propietarios.AddRange(ana, luis);
        db.Mascotas.AddRange(
            new Mascota { Nombre = "Luna", Especie = "Perro", Edad = 4, Propietario = ana },
            new Mascota { Nombre = "Milo", Especie = "Gato", Edad = 2, Propietario = ana },
            new Mascota { Nombre = "Kira", Especie = "Perro", Edad = 6, Propietario = luis });
        await db.SaveChangesAsync();
    }
}
