using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Farma_api.Dto.Auth;
using Farma_api.Helpers;
using Farma_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Farma_api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
    {
        var response = new AuthResponse();
        try
        {
            response = await authService.Authenticate(request);
            if (!response.IsSuccess)
                BadRequest(response);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Msg = ex.Message;
            return StatusCode(500, response);
        }
    }

    [HttpPost("loginRefresh")]
    public async Task<ActionResult<AuthResponse>> LoginRefresh([FromBody] RefreshRequest request)
    {
        var response = new AuthResponse();
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var expiredToken = tokenHandler.ReadJwtToken(request.ExpiredToken);
            if (expiredToken.ValidTo.AddMinutes(-6) > DateTime.UtcNow)
            {
                response.Status = HttpStatusCode.Unauthorized;
                response.Msg = "Token no ha expirado";
                return BadRequest(response);
            }

            var userId =
                Guid.Parse(expiredToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value);
            response = await authService.RefreshToken(request, userId);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Msg = ex.Message;
            return StatusCode(500, response);
        }
    }

    [HttpPost("recover")]
    [ResponseCache(Duration = 30)]
    public async Task<ActionResult<EmptyResponse>> RecoverPassword([FromBody] Recover request)
    {
        var response = new EmptyResponse();
        try
        {
            var result = await authService.RecoverPassword(request.Email);
            if (!result)
            {
                response.Status = HttpStatusCode.BadRequest;
                response.ErrorMessage.Add("Correo no encontrado");
                return BadRequest(response);
            }
            response.Status = HttpStatusCode.NoContent;
            response.IsSuccess = true;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Status = HttpStatusCode.InternalServerError;
            response.ErrorMessage.Add(ex.Message);
            return StatusCode(500, response);
        }
    }
}