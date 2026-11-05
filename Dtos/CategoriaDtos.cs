using System.ComponentModel.DataAnnotations;

namespace InventarioApi.Dtos;

public record CategoriaRequest(
    [Required, StringLength(80, MinimumLength = 3)] string Nombre,
    [StringLength(250)] string? Descripcion);

public record CategoriaResponse(int Id, string Nombre, string? Descripcion, int CantidadProductos);
