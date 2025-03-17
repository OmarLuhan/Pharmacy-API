namespace Farma_api.Dto.Sales;

public class SaleCreateDto
{
    public Guid UsuarioId { get; set; }

    public string? ClienteDni { get; set; }

    public string? ClienteNombre { get; set; }

    public virtual ICollection<SaleDetailCreateDto> DetalleVenta { get; set; } = [];
}