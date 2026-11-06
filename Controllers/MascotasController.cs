using Lab12NetApi.Data;
using Lab12NetApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab12NetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MascotasController(VeterinariaContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Mascota>>> GetMascotas(
        [FromQuery] string? especie,
        [FromQuery] int? edadMinima,
        [FromQuery] int? edadMaxima)
    {
        var consulta = context.Mascotas.AsNoTracking().Include(m => m.Propietario).AsQueryable();
        if (!string.IsNullOrWhiteSpace(especie))
            consulta = consulta.Where(m => m.Especie == especie);
        if (edadMinima.HasValue)
            consulta = consulta.Where(m => m.Edad >= edadMinima.Value);
        if (edadMaxima.HasValue)
            consulta = consulta.Where(m => m.Edad <= edadMaxima.Value);
        return Ok(await consulta.OrderBy(m => m.Nombre).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Mascota>> GetMascota(int id)
    {
        var mascota = await context.Mascotas.AsNoTracking()
            .Include(m => m.Propietario)
            .FirstOrDefaultAsync(m => m.Id == id);
        return mascota is null ? NotFound() : Ok(mascota);
    }

    [HttpPost]
    public async Task<ActionResult<Mascota>> PostMascota(Mascota mascota)
    {
        if (!await context.Propietarios.AnyAsync(p => p.Id == mascota.PropietarioId))
            return BadRequest(new { mensaje = "El propietario indicado no existe." });
        if (await context.Mascotas.AnyAsync(m => m.PropietarioId == mascota.PropietarioId && m.Nombre == mascota.Nombre))
            return Conflict(new { mensaje = "Ese propietario ya tiene una mascota con el mismo nombre." });

        mascota.Propietario = null;
        context.Mascotas.Add(mascota);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMascota), new { id = mascota.Id }, mascota);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutMascota(int id, Mascota mascota)
    {
        if (id != mascota.Id) return BadRequest(new { mensaje = "El id de la ruta no coincide con el cuerpo." });
        if (!await context.Mascotas.AnyAsync(m => m.Id == id)) return NotFound();
        if (!await context.Propietarios.AnyAsync(p => p.Id == mascota.PropietarioId))
            return BadRequest(new { mensaje = "El propietario indicado no existe." });
        if (await context.Mascotas.AnyAsync(m => m.PropietarioId == mascota.PropietarioId && m.Nombre == mascota.Nombre && m.Id != id))
            return Conflict(new { mensaje = "Ese propietario ya tiene una mascota con el mismo nombre." });

        mascota.Propietario = null;
        context.Entry(mascota).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMascota(int id)
    {
        var mascota = await context.Mascotas.FindAsync(id);
        if (mascota is null) return NotFound();

        context.Mascotas.Remove(mascota);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
