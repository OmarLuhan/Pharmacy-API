using System.Net.Http.Headers;
using Farma_api.Models;
using Microsoft.EntityFrameworkCore;

namespace Farma_api.Repository;

public interface ISaleRepository : IGenericRepository<Venta>
{
    Task<Venta> AddSaleAsync(Venta sale);
    Task<bool> ReverseSaleAsync(int saleId);
}

public class SaleRepository(FarmadbContext context) : GenericRepository<Venta>(context), ISaleRepository
{
    private readonly FarmadbContext _context = context;

    public async Task<Venta> AddSaleAsync(Venta sale)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var correlative = await _context.Correlativos
                .FirstAsync(nc => nc.Gestion == "sales");

            correlative.UltimoNumero++;
            _context.Correlativos.Update(correlative);
            await _context.SaveChangesAsync();

            var saleNumber = correlative.UltimoNumero.ToString().PadLeft(correlative.CantidadDigitos, '0');
            sale.Correlativo = saleNumber;

            await _context.Ventas.AddAsync(sale);
            await _context.SaveChangesAsync();

            if (!await UpdateBatchStockAsync(sale.DetalleVenta, sale.Id))
                throw new Exception("No se pudo actualizar el stock de los lotes.");

            await transaction.CommitAsync();
            return sale;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> ReverseSaleAsync(int saleId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            List<StockAudit> stockAudits = await _context.StockAudits
                .Where(sa => sa.VentaId == saleId)
                .ToListAsync();
            if (stockAudits.Count == 0) return false;

            var batchIds = stockAudits.Select(sa => sa.LoteId).Distinct().ToList();
            List<Lote> batches = await _context.Lotes
                .Where(l => batchIds.Contains(l.Id))
                .ToListAsync();
            foreach (var audit in stockAudits)
            {
                var batch = batches.FirstOrDefault(b => b.Id == audit.LoteId);
                if (batch == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                batch.StockLote += audit.CantidadReducida;
                _context.Lotes.Update(batch);
            }

            var sale = await _context.Ventas.FindAsync(saleId);
            if (sale == null) throw new InvalidOperationException("Registro no encontrado");
            sale.Estado = false;
            _context.Ventas.Update(sale);
            _context.StockAudits.RemoveRange(stockAudits);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    private async Task<bool> UpdateBatchStockAsync(IEnumerable<DetalleVenta> saleDetail, int saleId)
    {
        var auditRecords = new List<StockAudit>();

        try
        {
            foreach (var sd in saleDetail)
            {
                List<Lote> batches = await _context.Lotes
                    .Where(l => l.ProductoId == sd.ProductoId
                                && l.StockLote > 0
                                && l.Activo == true)
                    .OrderBy(l => l.FechaVencimiento)
                    .ToListAsync();
                var remainingQuantity = sd.Cantidad;
                foreach (var batch in batches)
                {
                    if (remainingQuantity <= 0) break;

                    var quantityToBeDeducted = Math.Min(remainingQuantity, batch.StockLote);

                    batch.StockLote -= quantityToBeDeducted;
                    remainingQuantity -= quantityToBeDeducted;

                    auditRecords.Add(new StockAudit
                    {
                        VentaId = saleId,
                        ProductoId = sd.ProductoId,
                        LoteId = batch.Id,
                        CantidadReducida = quantityToBeDeducted,
                        Fecha = DateTime.Now
                    });

                    _context.Lotes.Update(batch);
                }
            }

            if (auditRecords.Count == 0)
                return false;
            await _context.StockAudits.AddRangeAsync(auditRecords);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}