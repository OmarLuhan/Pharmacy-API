namespace Farma_api.Dto.Auth;

public class RefreshRequest
{
    public string ExpiredToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}