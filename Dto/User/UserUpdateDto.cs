namespace Farma_api.Dto.User;

public class UserUpdateDto
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public int RolId { get; set; }
    public bool Activo { get; set; }
}