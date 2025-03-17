namespace Farma_api.Dto.Category;

public class CategoryDto
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaRegistro { get; set; }
}