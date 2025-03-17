namespace Farma_api.Dto.User;

public class UserCreateDto
{
    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public int RolId { get; set; }
}