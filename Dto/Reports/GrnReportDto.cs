namespace Farma_api.Dto.Reports;

public class GrnReportDto
{
    public int Id { get; set; }

    public DateTime FechaRegistro { get; set; }
    public string Correlativo { get; set; } = null!;
    public string Documento { get; set; } = null!;
    public string Proveedor { get; set; } = null!;

    public string NombreProducto { get; set; } = null!;

    public string LoteNumero { get; set; } = null!;

    public int Cantidad { get; set; }

    public decimal Precio { get; set; }

    public decimal SubTotal { get; set; }
    public bool Estado { get; set; }
}