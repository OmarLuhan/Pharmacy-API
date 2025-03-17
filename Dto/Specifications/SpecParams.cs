using System.ComponentModel.DataAnnotations;

namespace Farma_api.Dto.Specifications;

public class SpecParams
{
    [Required] [Range(1, 50)] public int PageNumber { get; set; }

    [Required] [Range(1, 50)] public int PageSize { get; set; }
}