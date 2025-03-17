namespace Farma_api.Models;

public class Usuario
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? UrlFoto { get; set; }

    public string? NombreFoto { get; set; }

    public string Clave { get; set; } = null!;

    public int RolId { get; set; }

    public bool? Activo { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<Entrada> Entrada { get; set; } = new List<Entrada>();

    public virtual Role Rol { get; set; } = null!;

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}