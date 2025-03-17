using System.Net;
using Farma_api.Dto.Profile;
using Farma_api.Helpers;
using Farma_api.Models;
using Farma_api.Repository;
using Microsoft.EntityFrameworkCore;

namespace Farma_api.Services;

public interface IProfileService
{
    Task<EmptyResponse> ChangePassword(ChangeRequest request);
    Task RemoveFile(Guid id);
    Task UpdateFile(Guid id, IFormFile file);
    Task<Usuario?> GetProfile(Guid id);
    Task UpdateProfile(Usuario profile);
}

public class ProfileService : IProfileService
{
    private readonly IFirebaseService _firebaseService;
    private readonly IGenericRepository<Usuario> _repo;

    public ProfileService(IGenericRepository<Usuario> repo, IFirebaseService firebaseService)
    {
        _repo = repo;
        _firebaseService = firebaseService;
    }

    public async Task<EmptyResponse> ChangePassword(ChangeRequest request)
    {
        var response = new EmptyResponse();
        var user = await _repo.GetAsync(u => u.Id == request.Id);
        if (user == null)
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Usuario no encontrado");
            return response;
        }

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Clave))
        {
            response.Status = HttpStatusCode.BadRequest;
            response.ErrorMessage.Add("Contraseña actual incorrecta");
            return response;
        }

        user.Clave = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        try
        {
            await _repo.UpdateAsync(user);
            response.Status = HttpStatusCode.OK;
            response.IsSuccess = true;
            return response;
        }
        catch
        {
            response.Status = HttpStatusCode.InternalServerError;
            response.ErrorMessage.Add("Error al cambiar la contraseña");
            return response;
        }
    }

    public async Task RemoveFile(Guid id)
    {
        try
        {
            var user = await _repo.GetAsync(u => u.Id == id) ?? throw new Exception("perfil no encontrado");
            if (string.IsNullOrEmpty(user.NombreFoto))
                throw new Exception("este perfil no tiene foto");
            await _firebaseService.DeleteStorage("users", user.NombreFoto);
            user.UrlFoto = "";
            user.NombreFoto = "";
            await _repo.UpdateAsync(user);
        }
        catch
        {
            throw;
        }
    }

    public async Task UpdateFile(Guid id, IFormFile file)
    {
        try
        {
            var user = await _repo.GetAsync(u => u.Id == id) ?? throw new Exception("perfil no encontrado");

            using var stream = file.OpenReadStream();
            string objectName, url;

            if (!string.IsNullOrEmpty(user.NombreFoto))
            {
                objectName = $"users/{user.NombreFoto}";
                url = await _firebaseService.UploadToFirebaseAsync(stream, objectName, file.ContentType);
                user.UrlFoto = url;
                await _repo.UpdateAsync(user);
            }
            var nameCode = Guid.NewGuid().ToString("N");
            var extension = Path.GetExtension(file.FileName);
            var imageName = string.Concat(nameCode, extension);
            objectName = $"users/{imageName}";
            url = await _firebaseService.UploadToFirebaseAsync(stream, objectName, file.ContentType);
            user.UrlFoto = url;
            user.NombreFoto = imageName;
            await _repo.UpdateAsync(user);
        }
        catch
        {
            throw;
        }
    }

    public async Task<Usuario?> GetProfile(Guid id)
    {
        return await _repo.Query(u => u.Id == id)
            .Include(r => r.Rol)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateProfile(Usuario profile)
    {
        await _repo.UpdateAsync(profile);
    }
}