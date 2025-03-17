using Farma_api.Models;
using Microsoft.EntityFrameworkCore;

namespace Farma_api.Repository;

public interface IGoodsReceivedRepository : IGenericRepository<Entrada>
{
    public Task<Entrada?> AddGoodsReceived(Entrada grn);
    public Task<bool> ReverseGoodsReceived(int grnId);
}

public class GoodsReceivedRepository(FarmadbContext context)
    : GenericRepository<Entrada>(context), IGoodsReceivedRepository
{
    private readonly FarmadbContext _context = context;

    public async Task<Entrada?> AddGoodsReceived(Entrada grn)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (!await UpdateBatchStockAsync(grn.DetalleEntrada))
                throw new DbUpdateException("Error al agregar lo productos a los lotes");
            var correlative = await _context.Correlativos
                .FirstAsync(nc => nc.Gestion == "Entries");
            correlative.UltimoNumero++;
            _context.Correlativos.Update(correlative);
            await _context.SaveChangesAsync();
            var grnNumber = correlative.UltimoNumero.ToString().PadLeft(correlative.CantidadDigitos, '0');
            grn.Correlativo = grnNumber;
            await _context.Entradas.AddAsync(grn);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return grn;
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
    }

    public async Task<bool> ReverseGoodsReceived(int entryId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var entryDetails = await _context.DetalleEntradas
                .Where(ed => ed.EntradaId == entryId)
                .Select(ed => new { ed.Cantidad, ed.LoteId })
                .ToListAsync();
            if (entryDetails.Count == 0)
            {
                await transaction.RollbackAsync();
                return false;
            }

            var batchIds = entryDetails.Select(ed => ed.LoteId).Distinct().ToList();
            var batches = await _context.Lotes
                .Where(l => batchIds.Contains(l.Id))
                .ToListAsync();
            foreach (var detail in entryDetails)
            {
                var batch = batches.FirstOrDefault(l => l.Id == detail.LoteId);
                if (batch == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                batch.StockLote -= detail.Cantidad;
                _context.Lotes.Update(batch);
            }

            var entry = await _context.Entradas.FindAsync(entryId) ?? throw new InvalidOperationException("Registro no encontrado");
            entry.Estado = false;
            _context.Entradas.Update(entry);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    private async Task<bool> UpdateBatchStockAsync(IEnumerable<DetalleEntrada> grnDetail)
    {
        try
        {
            foreach (var de in grnDetail)
            {
                var batch = await _context.Lotes.Where(det => det.Id == de.LoteId).FirstOrDefaultAsync();
                if (batch == null) return false;
                batch.StockLote += de.Cantidad;
                _context.Lotes.Update(batch);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}