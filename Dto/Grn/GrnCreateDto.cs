namespace Farma_api.Dto.Grn;

public class GrnCreateDto
{
    public string Documento { get; set; } = null!;

    public Guid UsuarioId { get; set; }

    public string? Proveedor { get; set; }

    public virtual ICollection<GrnDetailCreateDto> DetalleEntrada { get; set; } = [];
}