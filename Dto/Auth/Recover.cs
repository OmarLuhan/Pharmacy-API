using System.ComponentModel.DataAnnotations;

namespace Farma_api.Dto.Auth;

public class Recover
{
    [EmailAddress] public string Email { get; set; } = null!;
}