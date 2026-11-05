using InventarioApi.Data;
using InventarioApi.Dtos;
using InventarioApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IEnumerable<CategoriaResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaResponse>>> GetAll()
    {
        var categorias = await db.Categorias.AsNoTracking()
            .OrderBy(c => c.Nombre)
            .Select(c => new CategoriaResponse(c.Id, c.Nombre, c.Descripcion, c.Productos.Count))
            .ToListAsync();
        return Ok(categorias);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<CategoriaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaResponse>> GetById(int id)
    {
        var categoria = await db.Categorias.AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoriaResponse(c.Id, c.Nombre, c.Descripcion, c.Productos.Count))
            .SingleOrDefaultAsync();
        return categoria is null ? NotFound() : Ok(categoria);
    }

    [HttpPost]
    [ProducesResponseType<CategoriaResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoriaResponse>> Create(CategoriaRequest request)
    {
        var nombre = request.Nombre.Trim();
        if (await db.Categorias.AnyAsync(c => c.Nombre == nombre))
            return Conflict(new { mensaje = "Ya existe una categoría con ese nombre." });

        var categoria = new Categoria { Nombre = nombre, Descripcion = request.Descripcion?.Trim() };
        db.Categorias.Add(categoria);
        await db.SaveChangesAsync();

        var response = new CategoriaResponse(categoria.Id, categoria.Nombre, categoria.Descripcion, 0);
        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, CategoriaRequest request)
    {
        var categoria = await db.Categorias.FindAsync(id);
        if (categoria is null) return NotFound();

        var nombre = request.Nombre.Trim();
        if (await db.Categorias.AnyAsync(c => c.Id != id && c.Nombre == nombre))
            return Conflict(new { mensaje = "Ya existe otra categoría con ese nombre." });

        categoria.Nombre = nombre;
        categoria.Descripcion = request.Descripcion?.Trim();
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        var categoria = await db.Categorias.Include(c => c.Productos)
            .SingleOrDefaultAsync(c => c.Id == id);
        if (categoria is null) return NotFound();
        if (categoria.Productos.Count > 0)
            return Conflict(new { mensaje = "No se puede eliminar una categoría que contiene productos." });

        db.Categorias.Remove(categoria);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
