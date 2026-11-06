using Lab12NetApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab12NetApi.Data;

public class VeterinariaContext(DbContextOptions<VeterinariaContext> options) : DbContext(options)
{
    public DbSet<Propietario> Propietarios => Set<Propietario>();
    public DbSet<Mascota> Mascotas => Set<Mascota>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Propietario>()
            .HasIndex(p => p.Email)
            .IsUnique();

        modelBuilder.Entity<Mascota>()
            .HasOne(m => m.Propietario)
            .WithMany(p => p.Mascotas)
            .HasForeignKey(m => m.PropietarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
