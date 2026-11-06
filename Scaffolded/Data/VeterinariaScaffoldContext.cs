using System;
using System.Collections.Generic;
using Lab12NetApi.Scaffolded.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab12NetApi.Scaffolded.Data;

public partial class VeterinariaScaffoldContext : DbContext
{
    public VeterinariaScaffoldContext(DbContextOptions<VeterinariaScaffoldContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Mascota> Mascotas { get; set; }

    public virtual DbSet<Propietario> Propietarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mascota>(entity =>
        {
            entity.HasOne(d => d.Propietario).WithMany(p => p.Mascota)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mascotas_Propietarios");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
