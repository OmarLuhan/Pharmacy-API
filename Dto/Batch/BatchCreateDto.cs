namespace Farma_api.Dto.Batch;

public class BatchCreateDto
{
    public string NumeroLote { get; set; } = null!;

    public int ProductoId { get; set; }
    public DateOnly FechaProduccion { get; set; }

    public DateOnly FechaVencimiento { get; set; }
}