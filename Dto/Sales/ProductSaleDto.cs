namespace Farma_api.Dto.Sales;

public class ProductSaleDto
{
    public int Id { get; set; }
    public string CodigoEan13 { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Concentracion { get; set; }
    public string? Presentacion { get; set; }
    public decimal? Precio { get; set; }
    public int StockDisponible { get; set; }
}