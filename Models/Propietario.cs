using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lab12NetApi.Models;

public class Propietario
{
    public int Id { get; set; }

    [Required, StringLength(100, MinimumLength = 3)]
    public string Nombre { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Telefono { get; set; }

    [JsonIgnore]
    public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
}
