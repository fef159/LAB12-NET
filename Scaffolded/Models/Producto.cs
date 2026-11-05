using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventarioApi.Scaffolded.Models;

[Index("Sku", Name = "UQ_Productos_Sku", IsUnique = true)]
public partial class Producto
{
    [Key]
    public int Id { get; set; }

    [StringLength(120)]
    public string Nombre { get; set; } = null!;

    [StringLength(30)]
    public string Sku { get; set; } = null!;

    [Column(TypeName = "decimal(12, 2)")]
    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public int CategoriaId { get; set; }

    [ForeignKey("CategoriaId")]
    [InverseProperty("Productos")]
    public virtual Categoria Categoria { get; set; } = null!;
}
