using Farma_api.Dto.Batch;

namespace Farma_api.Dto.Product;

public sealed class ProductWithBatchDto
{
    public int Id { get; set; }
    public string CodigoEan13 { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Presentacion { get; set; }
    public int CategoriaId { get; set; }
    public string? CategoriaNombre { get; set; }
    public string? Concentracion { get; set; }
    public int StockTotal { get; set; }
    public float Precio { get; set; }
    public bool Especial { get; set; }
    public ICollection<BatchDto> Lotes { get; set; } = [];
}