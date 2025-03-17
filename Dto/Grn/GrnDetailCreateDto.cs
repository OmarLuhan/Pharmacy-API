namespace Farma_api.Dto.Grn;

public class GrnDetailCreateDto
{
    public int ProductoId { get; set; }
    public int LoteId { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }
}