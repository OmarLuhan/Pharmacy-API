namespace Farma_api.Dto.Profile;

public class ChangeRequest
{
    public Guid Id { get; set; }
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}