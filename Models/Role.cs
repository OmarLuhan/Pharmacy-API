namespace Farma_api.Models;

public class Role
{
    public int Id { get; set; }

    public string Valor { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}