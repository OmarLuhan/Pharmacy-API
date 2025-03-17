using System.ComponentModel.DataAnnotations;

namespace Farma_api.Dto.Auth;

public class AuthRequest
{
    [Required] [EmailAddress] public string Email { get; set; } = null!;

    [Required] public string Password { get; set; } = null!;
}