using Farma_api.Dto.Email;
using Farma_api.Models;
using Farma_api.Repository;
using Microsoft.EntityFrameworkCore;

namespace Farma_api.Services;

public interface IUserService
{
    Task<IEnumerable<Usuario>> List();
    Task<Usuario?> GetByGuid(Guid id, bool tracked = true);
    Task<Usuario?> Create(Usuario user);
    Task<bool> IsUnique(string email, Guid? guid = null);
    Task Update(Usuario user);
    Task Delete(Usuario user);
    Task<IEnumerable<Usuario>> Search(string value);
}

public class UserService(
    IGenericRepository<Usuario> repository,
    IConfiguration configuration,
    IEmailService emailService,
    IFirebaseService firebaseService)
    : IUserService
{
    public async Task<Usuario?> Create(Usuario user)
    {
        var clave = Guid.NewGuid().ToString("N")[..8];
        user.Correo = user.Correo.ToLower();
        var urlAction = configuration.GetSection("MailSettings:UrlAction").Value;
        var urlLogo = configuration.GetSection("MailSettings:Logo").Value;
        try
        {
            user.Clave = BCrypt.Net.BCrypt.HashPassword(clave);
            var filePath = Path.Combine("Templates", "Correos", "Index.html");
            var body = await File.ReadAllTextAsync(filePath);
            body = body.Replace("[nombre]", user.Nombre)
                .Replace("[usuario]", user.Correo)
                .Replace("[clave]", clave)
                .Replace("[urlActionBtn]", urlAction)
                .Replace("[urlAction]", urlAction)
                .Replace("[urlText]", urlAction)
                .Replace("[urlLogo]", urlLogo);

            var email = new EmailDto
            {
                For = user.Correo,
                Subject = "Bienvenido a CrisFarma",
                Body = body
            };
            var send = await emailService.SendEmailAsync(email);
            if (!send) return null;
            var created = await repository.AddAsync(user);
            return created;
        }
        catch
        {
            return null;
        }
    }

    public async Task Delete(Usuario user)
    {
        const string folder = "profiles";
        var imageName = user.NombreFoto;
        await repository.DeleteAsync(user);
        if (!string.IsNullOrEmpty(imageName))
            await firebaseService.DeleteStorage(folder, imageName);
    }


    public async Task<Usuario?> GetByGuid(Guid id, bool tracked = true)
    {
        return await repository.GetAsync(u => u.Id == id, tracked);
    }


    public async Task<bool> IsUnique(string email, Guid? guid = null)
    {
        var usuario = await repository.GetAsync(u =>
            u.Correo == email.ToLower() &&
            (!guid.HasValue || u.Id != guid.Value));
        return usuario == null;
    }

    public async Task<IEnumerable<Usuario>> List()
    {
        IQueryable<Usuario> query = repository.Query()
            .Include(u => u.Rol);
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> Search(string value)
    {
        IQueryable<Usuario> query = repository.Query();
        query = query.Include(p => p.Rol)
            .Where(p => p.Nombre.Contains(value) ||
                        p.Correo.Contains(value) ||
                        p.Rol.Valor.Contains(value)).Take(10);
        return await query.ToListAsync();
    }

    public async Task Update(Usuario user)
    {
        await repository.UpdateAsync(user);
    }
}