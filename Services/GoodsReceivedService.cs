using Farma_api.Dto.Specifications;
using Farma_api.Models;
using Farma_api.Repository;
using Microsoft.EntityFrameworkCore;

namespace Farma_api.Services;

public interface IGoodsReceivedService
{
    Task<Entrada?> Register(Entrada grn);
    Task<bool> IsReversible(int grnId);
    Task<bool> ReverseGrn(int grnId);
    Task<Entrada?> Detail(string grnNumber);
     Task<IEnumerable<Entrada>> Search(string value);
    Task<PageList<Entrada>> GetGoodsReceived(SpecParams specParams, SpecFilters specFilters);
}

public class GoodsReceivedService(
    IGoodsReceivedRepository repo,
    IGenericRepository<Producto> productRepo,
    IGenericRepository<Lote> batchRepo)
    : IGoodsReceivedService
{
    private readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

    public async Task<Entrada?> Detail(string grnNumber)
    {
        var query = repo.Query(v => v.Correlativo == grnNumber.Trim());
        query = query.Include(v => v.DetalleEntrada)
            .Include(u => u.Usuario);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<bool> IsReversible(int grnId)
    {
        var grnDate = await repo.Query(s => s.Id == grnId && s.Estado != false)
            .Select(s => s.FechaRegistro)
            .FirstOrDefaultAsync();
        var daysSinceEntry = (DateTime.Now - grnDate).TotalHours;
        return !(daysSinceEntry > 48);
    }

    public async Task<bool> ReverseGrn(int grnId)
    {
        return await repo.ReverseGoodsReceived(grnId);
    }

    public Task<PageList<Entrada>> GetGoodsReceived(SpecParams specParams, SpecFilters specFilters)
    {
        var query = repo.Query();
        var startDate = specFilters.StartDate;
        var endDate = specFilters.EndDate;
        if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
        {
            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);
            start = TimeZoneInfo.ConvertTimeFromUtc(start, _timeZone);
            end = TimeZoneInfo.ConvertTimeFromUtc(end, _timeZone).AddDays(1);
            query = query.Where(v => v.FechaRegistro >= start && v.FechaRegistro < end)
                .OrderByDescending(v => v.Correlativo);
        }
        return PageList<Entrada>.ToPageList(query, specParams.PageNumber,
            specParams.PageSize);
    }

    public async Task<Entrada?> Register(Entrada grn)
    {
        try
        {
            if (string.IsNullOrEmpty(grn.Proveedor)) grn.Proveedor = "Proveedor anonimo";
            var productIds = grn.DetalleEntrada.Select(dv => dv.ProductoId).ToList();
            var products = await productRepo.Query(p => productIds.Contains(p.Id))
                .Select(p => new { p.Id, p.Nombre, p.CodigoEan13 })
                .ToListAsync();
            var batchIds = grn.DetalleEntrada.Select(dv => dv.LoteId).ToList();
            List<Lote> batches = await batchRepo.Query(b => batchIds.Contains(b.Id))
                .ToListAsync();
            if (products.Count <= 0 || products.Count != productIds.Count) return null;

            foreach (var detail in grn.DetalleEntrada)
            {
                var product = products.FirstOrDefault(p => p.Id == detail.ProductoId);
                if (product == null) return null;
                var batch = batches.FirstOrDefault(b => b.Id == detail.LoteId);
                if (batch == null) return null;
                detail.NombreProducto = product.Nombre;
                detail.LoteNumero = batch.NumeroLote;
                detail.CodigoProducto = product.CodigoEan13;
                detail.Total = detail.Precio * detail.Cantidad;
            }

            var taxBase = grn.DetalleEntrada.Sum(p => p.Total);
            var totalTax = Math.Round(taxBase * 0.18m, 2);
            var costPrice = Math.Round(taxBase + totalTax, 2);
            grn.Total = costPrice;
            grn.SubTotal = taxBase;
            grn.ImpuestoTotal = totalTax;
            var grnCreated = await repo.AddGoodsReceived(grn);
            return grnCreated ?? null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<Entrada>> Search(string value)
    {
       var grn= repo.Query(g=>g.Correlativo.Contains(value) || g.Proveedor.Contains(value)
                            || g.Documento.Contains(value)).Take(10);
        return await grn.ToListAsync();
    }
}