using Lab12NetApi.Data;
using Lab12NetApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab12NetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropietariosController(VeterinariaContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Propietario>>> GetPropietarios() =>
        Ok(await context.Propietarios.AsNoTracking().OrderBy(p => p.Nombre).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Propietario>> GetPropietario(int id)
    {
        var propietario = await context.Propietarios.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return propietario is null ? NotFound() : Ok(propietario);
    }

    [HttpGet("{id:int}/mascotas")]
    public async Task<ActionResult<IEnumerable<Mascota>>> GetMascotas(int id)
    {
        if (!await context.Propietarios.AnyAsync(p => p.Id == id)) return NotFound();
        return Ok(await context.Mascotas.AsNoTracking()
            .Where(m => m.PropietarioId == id)
            .OrderBy(m => m.Nombre)
            .ToListAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Propietario>> PostPropietario(Propietario propietario)
    {
        if (await context.Propietarios.AnyAsync(p => p.Email == propietario.Email))
            return Conflict(new { mensaje = "Ya existe un propietario con ese correo electrónico." });

        context.Propietarios.Add(propietario);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPropietario), new { id = propietario.Id }, propietario);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutPropietario(int id, Propietario propietario)
    {
        if (id != propietario.Id) return BadRequest(new { mensaje = "El id de la ruta no coincide con el cuerpo." });
        if (!await context.Propietarios.AnyAsync(p => p.Id == id)) return NotFound();
        if (await context.Propietarios.AnyAsync(p => p.Email == propietario.Email && p.Id != id))
            return Conflict(new { mensaje = "Ya existe un propietario con ese correo electrónico." });

        context.Entry(propietario).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePropietario(int id)
    {
        var propietario = await context.Propietarios.FindAsync(id);
        if (propietario is null) return NotFound();
        if (await context.Mascotas.AnyAsync(m => m.PropietarioId == id))
            return Conflict(new { mensaje = "No se puede eliminar un propietario que tiene mascotas registradas." });

        context.Propietarios.Remove(propietario);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
