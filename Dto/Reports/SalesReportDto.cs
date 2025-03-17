namespace Farma_api.Dto.Reports;

public class SalesReportDto
{
    public int Id { get; set; }

    public DateTime FechaRegistro { get; set; }
    public string Correlativo { get; set; } = null!;
    public string Documento { get; set; } = null!;
    public bool Estado { get; set; }
    public string ClienteNombre { get; set; } = null!;
    public string ClienteDni { get; set; } = null!;

    public string NombreProducto { get; set; } = null!;

    public int Cantidad { get; set; }

    public decimal Precio { get; set; }
    public decimal SubTotal { get; set; }
}