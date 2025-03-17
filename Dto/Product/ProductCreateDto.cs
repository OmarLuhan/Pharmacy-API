﻿namespace Farma_api.Dto.Product;

public class ProductCreateDto
{
    public string CodigoEan13 { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public int CategoriaId { get; set; }
    public string? Presentacion { get; set; }
    public string? Concentracion { get; set; }
    public float Precio { get; set; }
    public bool Especial { get; set; }
    public virtual ICollection<ProductBatchCreateDto> Lotes { get; set; } = [];
}