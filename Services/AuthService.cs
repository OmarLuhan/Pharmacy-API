using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Farma_api.Dto.Auth;
using Farma_api.Dto.Email;
using Farma_api.Models;
using Farma_api.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Farma_api.Services;

public interface IAuthService
{
    Task<bool> RecoverPassword(string email);
    Task<AuthResponse> Authenticate(AuthRequest auth);
    Task<AuthResponse> RefreshToken(RefreshRequest refresh, Guid userId);
}

public class AuthService(
    IGenericRepository<Usuario> repository,
    IConfiguration config,
    IGenericRepository<RefreshToken> repoRefresh,
    IEmailService emailService)
    : IAuthService
{
    public async Task<bool> RecoverPassword(string email)
    {
        var user = await repository.GetAsync(ur => ur.Correo == email.ToLower());
        if (user == null)
            return false;
        var urlAction = config.GetSection("MailSettings:UrlAction").Value;
        var urlLogo = config.GetSection("MailSettings:Logo").Value;
        var newPassword = Guid.NewGuid().ToString("N")[..8];
        user.Clave = BCrypt.Net.BCrypt.HashPassword(newPassword);
        try
        {
            var filePath = Path.Combine("Templates", "Correos", "Recover.html");
            var body = await File.ReadAllTextAsync(filePath);
            body = body.Replace("[nombre]", user.Nombre)
                .Replace("[usuario]", user.Correo)
                .Replace("[clave]", newPassword)
                .Replace("[urlActionBtn]", urlAction)
                .Replace("[urlAction]", urlAction)
                .Replace("[urlText]", urlAction)
                .Replace("[urlLogo]", urlLogo);
            var emailSend = new EmailDto
            {
                For = user.Correo,
                Subject = "Recuperación de contraseña",
                Body = body
            };
            var send = await emailService.SendEmailAsync(emailSend);
            if (!send)
                return false;
            await repository.UpdateAsync(user);
            return true;
        }
        catch
        {
            throw;
        }
    }

    public async Task<AuthResponse> Authenticate(AuthRequest auth)
    {
        var response = new AuthResponse();
        try
        {
            var user = await repository.Query(ur => ur.Correo == auth.Email)
                .Include(r => r.Rol)
                .FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(auth.Password, user.Clave))
            {
                response.Status = HttpStatusCode.Unauthorized;
                response.Msg = "Usuario o contraseña incorrectos";
                return response;
            }

            if (user.Activo != true)
            {
                response.Status = HttpStatusCode.Forbidden;
                response.Msg = "Usuario inactivo";
                return response;
            }

            var refreshGenerated = GenerateRefreshToken();
            var tokenGenerated = GenerateToken(user);
            if (refreshGenerated == string.Empty || tokenGenerated == string.Empty)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.Msg = "Error al generar token";
                return response;
            }

            await SaveTokens(user, refreshGenerated, tokenGenerated);
            response.Status = HttpStatusCode.OK;
            response.Token = tokenGenerated;
            response.TokenRefresh = refreshGenerated;
            response.Name = user.Nombre;
            response.ProfileUrl = user.UrlFoto;
            response.IsSuccess = true;
            return response;
        }
        catch (Exception ex)
        {
            response.Status = HttpStatusCode.InternalServerError;
            response.Msg = "Error to authenticate" + ex.Message;
            return response;
        }
    }

    public async Task<AuthResponse> RefreshToken(RefreshRequest refresh, Guid userId)
    {
        var response = new AuthResponse();
        try
        {
            var token = await repoRefresh.Query(rt => rt.Token == refresh.ExpiredToken
                                                      && rt.TokenRefresh == refresh.RefreshToken
                                                      && rt.UsuarioId == userId)
                .FirstOrDefaultAsync();
            var user = await repository.Query(urs => urs.Id == userId)
                .Include(r => r.Rol)
                .FirstOrDefaultAsync();
            if (token == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.Msg = "Token not found";
                return response;
            }

            if (user == null)
            {
                response.Status = HttpStatusCode.NotFound;
                response.Msg = "User not found";
                return response;
            }

            if (user.Activo != true)
            {
                response.Status = HttpStatusCode.Forbidden;
                response.Msg = "Inactive user";
                return response;
            }

            if (token.FechaExpiracion < DateTime.UtcNow)
            {
                response.Status = HttpStatusCode.Unauthorized;
                response.Msg = "RefreshToken expired";
                return response;
            }

            var refreshGenerated = GenerateRefreshToken();
            var tokenGenerated = GenerateToken(user);
            await SaveTokens(user, refreshGenerated, tokenGenerated);
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            response.Name = user.Nombre;
            response.Token = tokenGenerated;
            response.TokenRefresh = refreshGenerated;
            return response;
        }
        catch
        {
            response.Status = HttpStatusCode.InternalServerError;
            response.Msg = "Error al refrescar token";
            return response;
        }
    }

    private string GenerateToken(Usuario user)
    {
        var key = config.GetValue<string>("JwtSettings:SecretKey");
        if (key == null)
            return string.Empty;

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Nombre),
                    new Claim(ClaimTypes.Email, user.Correo),
                    new Claim(ClaimTypes.Role, user.Rol.Valor)
                ]),
                Expires = DateTime.UtcNow.AddHours(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey)
                    , SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string GenerateRefreshToken()
    {
        try
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task SaveTokens(Usuario user, string refreshToken, string token)
    {
        var refreshTokenModel = new RefreshToken
        {
            UsuarioId = user.Id,
            Token = token,
            TokenRefresh = refreshToken,
            FechaCreacion = DateTime.UtcNow,
            FechaExpiracion = DateTime.UtcNow.AddHours(24)
        };
        await repoRefresh.AddAsync(refreshTokenModel);
    }
}