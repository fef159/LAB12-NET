using InventarioApi.Data;
using InventarioApi.Dtos;
using InventarioApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IEnumerable<ProductoResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductoResponse>>> GetAll(int? categoriaId, string? buscar)
    {
        var consulta = db.Productos.AsNoTracking().Include(p => p.Categoria).AsQueryable();

        if (categoriaId.HasValue)
            consulta = consulta.Where(p => p.CategoriaId == categoriaId.Value);
        if (!string.IsNullOrWhiteSpace(buscar))
            consulta = consulta.Where(p => p.Nombre.Contains(buscar) || p.Sku.Contains(buscar));

        var productos = await consulta
            .OrderBy(p => p.Nombre)
            .Select(p => new ProductoResponse(
                p.Id, p.Nombre, p.Sku, p.Precio, p.Stock,
                p.CategoriaId, p.Categoria!.Nombre))
            .ToListAsync();
        return Ok(productos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<ProductoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoResponse>> GetById(int id)
    {
        var producto = await db.Productos.AsNoTracking().Include(p => p.Categoria)
            .SingleOrDefaultAsync(p => p.Id == id);
        return producto is null ? NotFound() : Ok(Map(producto));
    }

    [HttpGet("/api/categorias/{categoriaId:int}/productos")]
    [ProducesResponseType<IEnumerable<ProductoResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProductoResponse>>> GetByCategoria(int categoriaId)
    {
        if (!await db.Categorias.AnyAsync(c => c.Id == categoriaId)) return NotFound();
        var productos = await db.Productos.AsNoTracking().Include(p => p.Categoria)
            .Where(p => p.CategoriaId == categoriaId)
            .Select(p => new ProductoResponse(
                p.Id, p.Nombre, p.Sku, p.Precio, p.Stock,
                p.CategoriaId, p.Categoria!.Nombre))
            .ToListAsync();
        return Ok(productos);
    }

    [HttpPost]
    [ProducesResponseType<ProductoResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductoResponse>> Create(ProductoRequest request)
    {
        if (!await db.Categorias.AnyAsync(c => c.Id == request.CategoriaId))
            return BadRequest(new { mensaje = "La categoría indicada no existe." });

        var sku = request.Sku.Trim().ToUpperInvariant();
        if (await db.Productos.AnyAsync(p => p.Sku == sku))
            return Conflict(new { mensaje = "El SKU ya está registrado." });

        var producto = new Producto
        {
            Nombre = request.Nombre.Trim(),
            Sku = sku,
            Precio = request.Precio,
            Stock = request.Stock,
            CategoriaId = request.CategoriaId
        };
        db.Productos.Add(producto);
        await db.SaveChangesAsync();
        await db.Entry(producto).Reference(p => p.Categoria).LoadAsync();
        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, Map(producto));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, ProductoRequest request)
    {
        var producto = await db.Productos.FindAsync(id);
        if (producto is null) return NotFound();
        if (!await db.Categorias.AnyAsync(c => c.Id == request.CategoriaId))
            return BadRequest(new { mensaje = "La categoría indicada no existe." });

        var sku = request.Sku.Trim().ToUpperInvariant();
        if (await db.Productos.AnyAsync(p => p.Id != id && p.Sku == sku))
            return Conflict(new { mensaje = "El SKU ya está registrado por otro producto." });

        producto.Nombre = request.Nombre.Trim();
        producto.Sku = sku;
        producto.Precio = request.Precio;
        producto.Stock = request.Stock;
        producto.CategoriaId = request.CategoriaId;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var producto = await db.Productos.FindAsync(id);
        if (producto is null) return NotFound();
        db.Productos.Remove(producto);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static ProductoResponse Map(Producto p) =>
        new(p.Id, p.Nombre, p.Sku, p.Precio, p.Stock, p.CategoriaId, p.Categoria?.Nombre ?? string.Empty);
}
