namespace Farma_api.Dto.Batch;

public class BatchUpdateDto
{
    public int Id { get; set; }

    public string NumeroLote { get; set; } = null!;
    public int StockLote { get; set; }

    public bool Activo { get; set; }

    public DateOnly FechaProduccion { get; set; }

    public DateOnly FechaVencimiento { get; set; }
}