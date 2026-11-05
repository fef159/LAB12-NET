using System.ComponentModel.DataAnnotations;

namespace InventarioApi.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required, StringLength(80, MinimumLength = 3)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Descripcion { get; set; }

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
