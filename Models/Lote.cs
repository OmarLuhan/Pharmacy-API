namespace Farma_api.Models;

public class Lote
{
    public int Id { get; set; }

    public string NumeroLote { get; set; } = null!;

    public int ProductoId { get; set; }

    public int StockLote { get; set; }

    public bool? Activo { get; set; }

    public DateOnly FechaProduccion { get; set; }

    public DateOnly FechaVencimiento { get; set; }

    public virtual Producto Producto { get; set; } = null!;

    public virtual ICollection<StockAudit> StockAudits { get; set; } = new List<StockAudit>();
}