namespace Farma_api.Dto.Batch;

public class BatchDto
{
    public int Id { get; set; }
    public string NumeroLote { get; set; } = null!;
    public DateOnly FechaVencimiento { get; set; }
    public DateOnly FechaProduccion { get; set; }
    public int StockLote { get; set; }
    public bool Activo { get; set; }
}