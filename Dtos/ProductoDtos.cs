using System.ComponentModel.DataAnnotations;

namespace InventarioApi.Dtos;

public record ProductoRequest(
    [Required, StringLength(120, MinimumLength = 3)] string Nombre,
    [Required, StringLength(30, MinimumLength = 3)] string Sku,
    [Range(0.01, 999999999)] decimal Precio,
    [Range(0, 1000000)] int Stock,
    [Range(1, int.MaxValue)] int CategoriaId);

public record ProductoResponse(
    int Id, string Nombre, string Sku, decimal Precio, int Stock,
    int CategoriaId, string Categoria);
