namespace Farma_api.Models;

public class Venta
{
    public int Id { get; set; }

    public string Correlativo { get; set; } = null!;

    public string Documento { get; set; } = null!;

    public Guid UsuarioId { get; set; }

    public string ClienteDni { get; set; } = null!;

    public string ClienteNombre { get; set; } = null!;

    public decimal SubTotal { get; set; }

    public decimal ImpuestoTotal { get; set; }

    public decimal Total { get; set; }

    public bool? Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public virtual ICollection<StockAudit> StockAudits { get; set; } = new List<StockAudit>();

    public virtual Usuario Usuario { get; set; } = null!;
}