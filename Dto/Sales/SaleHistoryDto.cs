namespace Farma_api.Dto.Sales;

public class SaleHistoryDto
{
    public int Id { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Correlativo { get; set; } = null!;
    public string ClienteDni { get; set; } = null!;
    public string ClienteNombre { get; set; } = null!;
    public decimal Total { get; set; }
    public bool Estado { get; set; }
}