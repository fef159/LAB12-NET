using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Lab12NetApi.Scaffolded.Models;

public partial class Mascota
{
    [Key]
    public int Id { get; set; }

    [StringLength(80)]
    public string Nombre { get; set; } = null!;

    [StringLength(50)]
    public string Especie { get; set; } = null!;

    public int Edad { get; set; }

    public int PropietarioId { get; set; }

    [ForeignKey("PropietarioId")]
    [InverseProperty("Mascota")]
    public virtual Propietario Propietario { get; set; } = null!;
}
