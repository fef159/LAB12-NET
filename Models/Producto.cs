using System.ComponentModel.DataAnnotations;

namespace InventarioApi.Models;

public class Producto
{
    public int Id { get; set; }

    [Required, StringLength(120, MinimumLength = 3)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(30, MinimumLength = 3)]
    public string Sku { get; set; } = string.Empty;

    [Range(0.01, 999999999)]
    public decimal Precio { get; set; }

    [Range(0, 1000000)]
    public int Stock { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }
}
