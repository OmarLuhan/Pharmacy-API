namespace Farma_api.Models;

public class Entrada
{
    public int Id { get; set; }

    public string Correlativo { get; set; } = null!;

    public string Documento { get; set; } = null!;

    public Guid UsuarioId { get; set; }

    public string Proveedor { get; set; } = null!;

    public decimal SubTotal { get; set; }

    public decimal ImpuestoTotal { get; set; }

    public decimal Total { get; set; }

    public bool? Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<DetalleEntrada> DetalleEntrada { get; set; } = new List<DetalleEntrada>();

    public virtual Usuario Usuario { get; set; } = null!;
}