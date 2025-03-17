namespace Farma_api.Models;

public class Producto
{
    public int Id { get; set; }

    public string CodigoEan13 { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public int? CategoriaId { get; set; }

    public string? Presentacion { get; set; }

    public string? Concentracion { get; set; }

    public float Precio { get; set; }

    public bool Especial { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual Categoria? Categoria { get; set; }

    public virtual ICollection<Lote> Lotes { get; set; } = new List<Lote>();
}