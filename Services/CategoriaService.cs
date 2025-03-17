using Farma_api.Dto.Specifications;
using Farma_api.Models;
using Farma_api.Repository;
using Microsoft.EntityFrameworkCore;

namespace Farma_api.Services;

public interface ICategoryService
{
    Task<IEnumerable<Categoria>> List();
    Task<PageList<Categoria>> List(SpecParams specParams);
    Task<Categoria?> GetById(int id);
    Task<Categoria?> Create(Categoria category);
    Task Update(Categoria category);
    Task Delete(Categoria category);
    Task<bool> IsUnique(string description, int? id = null);
    Task<IEnumerable<Categoria>> Search(string value);
}

public class CategoryService(IGenericRepository<Categoria> repository) : ICategoryService
{
    public async Task<IEnumerable<Categoria>> List()
    {
        IQueryable<Categoria> categories = repository.Query
                (c => c.Activo != false)
            .OrderBy(c => c.Descripcion);
        return await categories.ToListAsync();
    }

    public async Task<PageList<Categoria>> List(SpecParams specParams)
    {
        IQueryable<Categoria> categories = repository.Query().OrderByDescending(c => c.Id);
        return await PageList<Categoria>.ToPageList(categories, specParams.PageNumber,
            specParams.PageSize);
    }

    public async Task<Categoria?> GetById(int id)
    {
        return await repository.GetAsync(c => c.Id == id);
    }

    public async Task<Categoria?> Create(Categoria category)
    {
        return await repository.AddAsync(category);
    }

    public async Task Update(Categoria category)
    {
        await repository.UpdateAsync(category);
    }

    public async Task Delete(Categoria category)
    {
        await repository.DeleteAsync(category);
    }

    public async Task<bool> IsUnique(string description, int? id = null)
    {
        var category = await repository.Query(c => c.Descripcion.ToLower() == description.ToLower()
                                                   && (!id.HasValue || c.Id != id.Value))
            .Select(c => new { c.Id })
            .FirstOrDefaultAsync();
        return category == null;
    }

    public async Task<IEnumerable<Categoria>> Search(string value)
    {
        IQueryable<Categoria> categories = repository.Query
            (c => c.Descripcion.Contains(value)).Take(10);
        return await categories.ToListAsync();
    }
}