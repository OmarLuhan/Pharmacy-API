namespace Farma_api.Dto.Profile;

public class ProfileDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public int RolId { get; set; }
    public string? UrlFoto { get; set; }
    public string RolNombre { get; set; } = null!;
    public bool? Activo { get; set; }
    public DateTime? FechaCreacion { get; set; }
}