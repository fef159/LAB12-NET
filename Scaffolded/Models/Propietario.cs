using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lab12NetApi.Scaffolded.Models;

[Index("Email", Name = "UQ_Propietarios_Email", IsUnique = true)]
public partial class Propietario
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? Telefono { get; set; }

    [InverseProperty("Propietario")]
    public virtual ICollection<Mascota> Mascota { get; set; } = new List<Mascota>();
}
