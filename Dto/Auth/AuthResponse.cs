using System.Net;

namespace Farma_api.Dto.Auth;

public class AuthResponse
{
    public HttpStatusCode Status { set; get; }
    public bool IsSuccess { get; set; }
    public string? Name { get; set; }
    public string? Token { get; set; }
    public string? TokenRefresh { get; set; }
    public string? Msg { get; set; }
    public string? ProfileUrl { get; set; }
}