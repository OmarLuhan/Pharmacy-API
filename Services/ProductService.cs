using Farma_api.Dto.Specifications;
using Farma_api.Models;
using Farma_api.Repository;
using Microsoft.EntityFrameworkCore;

namespace Farma_api.Services;

public interface IProductService
{
    Task<PageList<Producto>> List(SpecParams specParams);
    Task<IEnumerable<Producto>> List();
    Task<Producto?> ProductForSale(string code);
    Task<IEnumerable<Producto>?> ProductForSaleName(string code);
    Task<IEnumerable<Producto>> Search(string value);
    Task<Producto?> GetById(int id, bool tracked = true);
    Task<Producto?> GetByCode(string code);
    Task<IEnumerable<Producto>?> ProductByName(string name);
    Task<Producto?> Create(Producto product);
    Task Update(Producto product);
    Task Delete(Producto product);
    Task<bool> HasProducts(int categoryId);
    Task<bool> IsUnique(string code, int? id = null);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<PageList<Producto>> List(SpecParams specParams)
    {
        IQueryable<Producto> query = _repository.Query()
            .Include(c => c.Categoria)
            .Include(l => l.Lotes)
            .OrderByDescending(c => c.Id);
        return await PageList<Producto>.ToPageList(query, specParams.PageNumber,
            specParams.PageSize);
    }

    public async Task<IEnumerable<Producto>> List()
    {
        IEnumerable<Producto> list = await _repository.Query()
            .Include(c => c.Categoria)
            .Include(l => l.Lotes)
            .OrderBy(n => n.Nombre)
            .ToListAsync();
        return list;
    }

    public async Task<Producto?> ProductForSale(string code)
    {
        return await _repository.Query(p => p.CodigoEan13 == code.Trim()
                                            && p.Lotes.Where(l => l.Activo == true
                                                                  && l.FechaVencimiento >
                                                                  DateOnly.FromDateTime(DateTime.Now))
                                                .Sum(l => l.StockLote) > 0)
            .Include(p => p.Lotes)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Producto>?> ProductForSaleName(string code)
    {
        var products = await _repository.Query(
                p => p.Nombre.Contains(code.Trim()) &&
                     p.Lotes.Any(l => l.Activo == true &&
                                      l.FechaVencimiento > DateOnly.FromDateTime(DateTime.Now) &&
                                      l.StockLote > 0))
            .Include(p => p.Lotes)
            .Take(5)
            .ToListAsync();

        return products;
    }

    public async Task<Producto?> GetById(int id, bool tracked = true)
    {
        return await _repository.GetAsync(p => p.Id == id, tracked);
    }

    public async Task<Producto?> Create(Producto product)
    {
        return await _repository.AddProductAsync(product);
    }

    public async Task Update(Producto product)
    {
        await _repository.UpdateAsync(product);
    }


    public async Task Delete(Producto product)
    {
        await _repository.DeleteAsync(product);
    }


    public async Task<IEnumerable<Producto>> Search(string value)
    {
        IQueryable<Producto> query = _repository.Query(p =>
                p.CodigoEan13.Contains(value) ||
                p.Nombre.Contains(value) ||
                (p.Categoria != null && p.Categoria.Descripcion.Contains(value))
            ).Take(10)
            .Include(p => p.Categoria)
            .Include(p => p.Lotes);

        return await query.ToListAsync();
    }


    public async Task<bool> IsUnique(string code, int? id = null)
    {
        var product = await _repository.Query
            (p => p.CodigoEan13 == code.Trim() &&
                  (!id.HasValue || p.Id != id.Value))
            .Select(p => new { p.Id })
            .FirstOrDefaultAsync();
        return product == null;
    }

    public async Task<bool> HasProducts(int categoryId)
    {
        return await _repository.Query(p => p.CategoriaId == categoryId)
            .AnyAsync();
    }


    public async Task<Producto?> GetByCode(string code)
    {
        return await _repository.Query(p => p.CodigoEan13 == code.Trim())
            .Include(c => c.Categoria)
            .Include(l => l.Lotes).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Producto>?> ProductByName(string name)
    {
        var products = await _repository.Query(
                p => p.Nombre.Contains(name.Trim()))
            .Include(p => p.Lotes)
            .Take(5)
            .ToListAsync();
        return products;
    }
}