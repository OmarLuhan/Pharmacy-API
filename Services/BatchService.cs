using Farma_api.Dto.Specifications;
using Farma_api.Models;
using Farma_api.Repository;
using Microsoft.EntityFrameworkCore;

namespace Farma_api.Services;

public interface IBatchService
{
    Task<Lote?> BatchById(int id, bool tracked = true);
    Task<Lote?> Create(Lote batch);
    Task Update(Lote batch);
    Task Delete(Lote batch);
    Task<PageList<Lote>> BatchesProduct(int id, SpecParams specParams);
    Task<bool> HasBatches(int productId);
    Task<bool> IsUniqueBatch(int productId, string numberBatch);
}

public class BatchService : IBatchService
{
    private readonly IGenericRepository<Lote> _batchRepo;

    public BatchService(IGenericRepository<Lote> batchRepo)
    {
        _batchRepo = batchRepo;
    }

    public async Task<Lote?> Create(Lote batch)
    {
        return await _batchRepo.AddAsync(batch);
    }

    public async Task Delete(Lote batch)
    {
        await _batchRepo.DeleteAsync(batch);
    }


    public async Task<bool> HasBatches(int productId)
    {
        return await _batchRepo.Query()
            .AnyAsync(l => l.Producto.Id == productId);
    }

    public async Task<bool> IsUniqueBatch(int productId, string numberBatch)
    {
        var batchEntity = await _batchRepo.Query(l => l.Producto.Id == productId
                                                      && l.NumeroLote.ToLower() == numberBatch.ToLower())
            .FirstOrDefaultAsync();
        return batchEntity == null;
    }


    public async Task<Lote?> BatchById(int id, bool tracked = true)
    {
        return await _batchRepo.GetAsync(l => l.Id == id, tracked);
    }


    public async Task<PageList<Lote>> BatchesProduct(int id, SpecParams specParams)
    {
        IQueryable<Lote> query = _batchRepo.Query(l => l.Producto.Id == id)
            .OrderBy(l => l.FechaVencimiento);
        return await PageList<Lote>.ToPageList(query, specParams.PageNumber, specParams.PageSize);
    }

    public async Task Update(Lote batch)
    {
        await _batchRepo.UpdateAsync(batch);
    }
}