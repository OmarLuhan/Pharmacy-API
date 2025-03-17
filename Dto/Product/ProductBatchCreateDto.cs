namespace Farma_api.Dto.Product;

public class ProductBatchCreateDto
{
    public string NumeroLote { get; set; } = null!;

    public DateOnly FechaProduccion { get; set; }

    public DateOnly FechaVencimiento { get; set; }
}