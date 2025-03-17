namespace Farma_api.Dto.Profile;

public class ProfileUpdateDto
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;
}