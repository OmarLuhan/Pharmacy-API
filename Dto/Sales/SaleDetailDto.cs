namespace Farma_api.Dto.Sales;

public class SaleDetailDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }

    public string? NombreProducto { get; set; }

    public string CodigoProducto { get; set; } = null!;

    public int Cantidad { get; set; }

    public decimal? Precio { get; set; }

    public decimal? Total { get; set; }
}