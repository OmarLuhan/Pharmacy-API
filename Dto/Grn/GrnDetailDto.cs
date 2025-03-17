namespace Farma_api.Dto.Grn;

public class GrnDetailDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }

    public string CodigoProducto { get; set; } = null!;

    public string NombreProducto { get; set; } = null!;

    public int LoteId { get; set; }

    public string? LoteNumero { get; set; }

    public int Cantidad { get; set; }

    public decimal Precio { get; set; }

    public decimal Total { get; set; }
}