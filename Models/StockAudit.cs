namespace Farma_api.Models;

public class StockAudit
{
    public int Id { get; set; }

    public int VentaId { get; set; }

    public int ProductoId { get; set; }

    public int LoteId { get; set; }

    public int CantidadReducida { get; set; }

    public DateTime Fecha { get; set; }

    public virtual Lote Lote { get; set; } = null!;

    public virtual Venta Venta { get; set; } = null!;
}