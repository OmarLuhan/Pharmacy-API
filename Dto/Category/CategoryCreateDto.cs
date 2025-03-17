using System.ComponentModel.DataAnnotations;

namespace Farma_api.Dto.Category;

public class CategoryCreateDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Descripcion { get; set; } = null!;
}