namespace Farma_api.Dto.Grn;

public class GrnDto
{
    public int Id { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Correlativo { get; set; } = null!;

    public string Documento { get; set; } = null!;

    public string Proveedor { get; set; } = null!;

    public decimal SubTotal { get; set; }

    public decimal ImpuestoTotal { get; set; }

    public decimal Total { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<GrnDetailDto> DetalleEntrada { get; set; } = [];
}