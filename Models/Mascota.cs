using System.ComponentModel.DataAnnotations;

namespace Lab12NetApi.Models;

public class Mascota
{
    public int Id { get; set; }

    [Required, StringLength(80, MinimumLength = 2)]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Especie { get; set; } = string.Empty;

    [Range(0, 40)]
    public int Edad { get; set; }

    [Range(1, int.MaxValue)]
    public int PropietarioId { get; set; }

    public Propietario? Propietario { get; set; }
}
