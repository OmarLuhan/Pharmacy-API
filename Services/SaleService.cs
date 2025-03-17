using Farma_api.Dto.Specifications;
using Farma_api.Models;
using Farma_api.Repository;
using Microsoft.EntityFrameworkCore;

namespace Farma_api.Services;

public interface ISaleService
{
    Task<Venta> Create(Venta sale);
    Task<bool> IsReversible(int saleId);
    Task<bool> ReverseSale(int saleId);
    Task<bool> IsStockSufficient(IEnumerable<DetalleVenta> saleDetail);
    Task<Venta?> Detail(string saleNumber);
    Task<PageList<Venta>> GetSale(SpecFilters specFilters, SpecParams specParams);
    Task<bool> HasUserSale(Guid userId);
    Task<IEnumerable<Venta>> Search(string value);
}

public class SaleService(
    ISaleRepository repo,
    IGenericRepository<Producto> productRepo,
    IGenericRepository<Lote> batchRepo)
    : ISaleService
{
    private readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

    public async Task<Venta> Create(Venta sale)
    {
        if (string.IsNullOrEmpty(sale.ClienteDni))
            sale.ClienteDni = "00000000";

        if (string.IsNullOrEmpty(sale.ClienteNombre))
            sale.ClienteNombre = "Cliente Anónimo";

        try
        {
            var productIds = sale.DetalleVenta.Select(dv => dv.ProductoId).ToList();
            var products = await productRepo.Query(p => productIds.Contains(p.Id))
                .Select(p => new { p.Id, p.Nombre, p.Precio, p.CodigoEan13 }).ToListAsync();
            if (products.Count <= 0 || products.Count != productIds.Count) return null;
            foreach (var detail in sale.DetalleVenta)
            {
                var product = products.FirstOrDefault(p => p.Id == detail.ProductoId);
                if (product == null) return null;
                detail.Precio = (decimal)product.Precio;
                detail.NombreProducto = product.Nombre;
                detail.CodigoProducto = product.CodigoEan13;
                detail.Total = (decimal)(product.Precio * detail.Cantidad);
            }

            var sellingPrice = sale.DetalleVenta.Sum(dv => dv.Total);
            var taxBase = Math.Round(sellingPrice / 1.18m, 2);
            var totalTax = Math.Round(taxBase * 0.18m, 2);
            sale.Total = sellingPrice;
            sale.ImpuestoTotal = totalTax;
            sale.SubTotal = taxBase;
            var generatedSale = await repo.AddSaleAsync(sale);
            return generatedSale;
        }
        catch
        {
            throw;;
        }
    }

    public async Task<bool> IsReversible(int saleId)
    {
        var saleDate = await repo.Query(s => s.Id == saleId && s.Estado != false)
            .Select(s => s.FechaRegistro)
            .FirstOrDefaultAsync();
        var daysSinceSale = (DateTime.Now - saleDate).TotalDays;
        return !(daysSinceSale > 3);
    }

    public Task<bool> ReverseSale(int saleId)
    {
        return repo.ReverseSaleAsync(saleId);
    }

    public async Task<bool> IsStockSufficient(IEnumerable<DetalleVenta> saleDetail)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        try
        {
            foreach (var sd in saleDetail)
            {
                var batches = await batchRepo.Query(l => l.ProductoId == sd.ProductoId
                                                         && l.StockLote > 0
                                                         && l.FechaVencimiento > today
                                                         && l.Activo == true)
                    .ToListAsync();

                if (batches.Count == 0 || batches.Sum(l => l.StockLote) < sd.Cantidad) return false;
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<Venta?> Detail(string saleNumber)
    {
        var query = repo.Query(v => v.Correlativo == saleNumber.Trim());
        query = query.Include(v => v.DetalleVenta)
            .Include(u => u.Usuario);
        return await query.FirstOrDefaultAsync();
    }

    public Task<PageList<Venta>> GetSale(SpecFilters specFilters, SpecParams specParams)
    {
        var query = repo.Query();
        var startDate = specFilters.StartDate;
        var endDate = specFilters.EndDate;
        if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
        {
            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);
            start = TimeZoneInfo.ConvertTimeToUtc(start, _timeZone);
            end = TimeZoneInfo.ConvertTimeToUtc(end, _timeZone).AddDays(1);
            query = query.Where(v => v.FechaRegistro >= start && v.FechaRegistro < end)
                .OrderByDescending(v => v.Correlativo);
        }
        return PageList<Venta>.ToPageList(query, specParams.PageNumber,
            specParams.PageSize);
    }

    public Task<bool> HasUserSale(Guid userId)
    {
        try
        {
            return repo.Query(v => v.UsuarioId == userId).AnyAsync();
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<IEnumerable<Venta>> Search(string value)
    {
        var sale= repo.Query(s=>s.Correlativo.Contains(value) || s.ClienteNombre.Contains(value)
                            || s.ClienteDni.Contains(value)).Take(10);
        return await sale.ToListAsync();
    }
}