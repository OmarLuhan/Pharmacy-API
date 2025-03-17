namespace Farma_api.Models;

public class RefreshToken
{
    public int Id { get; set; }

    public Guid? UsuarioId { get; set; }

    public string? Token { get; set; }

    public string? TokenRefresh { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaExpiracion { get; set; }
}