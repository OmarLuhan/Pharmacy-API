namespace Farma_api.Dto.Grn;

public class GrnHistoryDto
{
    public int Id { get; set; }
    public DateTime FechaRegistro { get; set; }

    public string Correlativo { get; set; } = null!;

    public string Documento { get; set; } = null!;

    public string UsuarioNombre { get; set; } = null!;

    public string Proveedor { get; set; } = null!;

    public decimal Total { get; set; }

    public bool? Estado { get; set; }
}