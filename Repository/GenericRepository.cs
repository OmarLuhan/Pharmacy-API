using System.Linq.Expressions;
using Farma_api.Models;
using Microsoft.EntityFrameworkCore;

namespace Farma_api.Repository;

public interface IGenericRepository<T> where T : class
{
    Task<T> AddAsync(T entity);
    Task<T?> GetAsync(Expression<Func<T, bool>> filters, bool tracked = true);
    IQueryable<T> Query(Expression<Func<T, bool>>? filters = null);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly FarmadbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(FarmadbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> filters, bool tracked = true)
    {
        IQueryable<T> query = _dbSet;
        if (!tracked) query = query.AsNoTracking();
        return await query.Where(filters).FirstOrDefaultAsync();
    }

    public IQueryable<T> Query(Expression<Func<T, bool>>? filters = null)
    {
        IQueryable<T> query = filters == null ? _dbSet : _dbSet.Where(filters);
        return query;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }
}