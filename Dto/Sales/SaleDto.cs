namespace Farma_api.Dto.Sales;

public class SaleDto
{
    public int Id { get; set; }
    public string Correlativo { get; set; } = null!;
    public string UsuarioNombre { get; set; } = null!;
    public string ClienteDni { get; set; } = null!;
    public string ClienteNombre { get; set; } = null!;
    public decimal SubTotal { get; set; }
    public decimal ImpuestoTotal { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = null!;
    public DateTime FechaRegistro { get; set; }
    public virtual ICollection<SaleDetailDto> DetalleVenta { get; set; } = [];
}