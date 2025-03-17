namespace Farma_api.Models;

public class Categoria
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public bool? Activo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}