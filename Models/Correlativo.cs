namespace Farma_api.Models;

public class Correlativo
{
    public int Id { get; set; }

    public int UltimoNumero { get; set; }

    public int CantidadDigitos { get; set; }

    public string Gestion { get; set; } = null!;

    public DateTime FechaActualizacion { get; set; }
}