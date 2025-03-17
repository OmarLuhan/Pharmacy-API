using Farma_api.Models;

namespace Farma_api.Repository;

public interface IProductRepository : IGenericRepository<Producto>
{
    Task<Producto?> AddProductAsync(Producto product);
}

public class ProductRepository(FarmadbContext context) : GenericRepository<Producto>(context), IProductRepository
{
    private readonly FarmadbContext _context = context;

    public async Task<Producto?> AddProductAsync(Producto product)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Productos.AddAsync(product);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return product;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}