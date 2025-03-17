namespace Farma_api.Dto.Grn;

public class ProductGrnDto
{
    public int Id { get; set; }
    public string CodigoEan13 { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Concentracion { get; set; }
    public string? Presentacion { get; set; }
    public virtual ICollection<BatchGrnDto> Lotes { get; set; } = [];
}