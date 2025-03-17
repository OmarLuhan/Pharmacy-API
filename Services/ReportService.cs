using Farma_api.Dto.Reports;
using Farma_api.Dto.Specifications;
using Farma_api.Models;
using Farma_api.Repository;
using Microsoft.EntityFrameworkCore;

namespace Farma_api.Services;

public interface IReportService
{
    Task<PageList<DetalleVenta>> SalesReport(SpecParams specParams, DateFilter dateFilter);
    Task<IEnumerable<DetalleVenta>> DownloadSalesReport(DateFilter dateFilter);
    Task<PageList<DetalleEntrada>> EntriesReport(SpecParams specParams, DateFilter dateFilter);
    Task<IEnumerable<DetalleEntrada>> DownloadEntriesReport(DateFilter dateFilter);
}

public class ReportService : IReportService
{
    private readonly IGenericRepository<DetalleEntrada> _entryDetailRepository;

    private readonly IGenericRepository<DetalleVenta> _saleDetailRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly TimeZoneInfo _timeZone;

    public ReportService(IGenericRepository<DetalleVenta> saleDetailRepository,
        IGenericRepository<DetalleEntrada> entryDetailRepository, ISaleRepository saleRepository)
    {
        _saleDetailRepository = saleDetailRepository;
        _entryDetailRepository = entryDetailRepository;
        _saleRepository = saleRepository;
        _timeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
    }

    public async Task<PageList<DetalleVenta>> SalesReport(SpecParams specParams, DateFilter dateFilter)
    {
        var start = DateTime.Parse(dateFilter.StartDate);
        var end = DateTime.Parse(dateFilter.EndDate);
        start = TimeZoneInfo.ConvertTimeToUtc(start, _timeZone);
        end = TimeZoneInfo.ConvertTimeToUtc(end, _timeZone).AddDays(1);
        IQueryable<DetalleVenta> query = _saleDetailRepository.Query(dv => dv.Venta.FechaRegistro >= start
                                                                           && dv.Venta.FechaRegistro < end)
            .Include(dv => dv.Venta)
            .OrderByDescending(dv => dv.Venta.Correlativo);
        return await PageList<DetalleVenta>.ToPageList(query, specParams.PageNumber, specParams.PageSize);
    }

    public async Task<PageList<DetalleEntrada>> EntriesReport(SpecParams specParams, DateFilter dateFilter)
    {
        var start = DateTime.Parse(dateFilter.StartDate);
        var end = DateTime.Parse(dateFilter.EndDate);
        start = TimeZoneInfo.ConvertTimeToUtc(start, _timeZone);
        end = TimeZoneInfo.ConvertTimeToUtc(end, _timeZone).AddDays(1);
        IQueryable<DetalleEntrada> query = _entryDetailRepository.Query(de => de.Entrada.FechaRegistro >= start
                                                                              && de.Entrada.FechaRegistro < end)
            .Include(de => de.Entrada)
            .OrderByDescending(ent => ent.Entrada.FechaRegistro);
        return await PageList<DetalleEntrada>.ToPageList(query, specParams.PageNumber, specParams.PageSize);
    }

    public async Task<IEnumerable<DetalleVenta>> DownloadSalesReport(DateFilter dateFilter)
    {
        var start = DateTime.Parse(dateFilter.StartDate);
        var end = DateTime.Parse(dateFilter.EndDate);
        start = TimeZoneInfo.ConvertTimeToUtc(start, _timeZone);
        end = TimeZoneInfo.ConvertTimeToUtc(end, _timeZone).AddDays(1);
        IQueryable<DetalleVenta> query = _saleDetailRepository.Query(dv => dv.Venta.FechaRegistro >= start
                                                                           && dv.Venta.FechaRegistro < end)
            .Include(dv => dv.Venta).OrderByDescending(dv => dv.Venta.FechaRegistro);
        return await query.ToListAsync();
    }

    public async Task<IEnumerable<DetalleEntrada>> DownloadEntriesReport(DateFilter dateFilter)
    {
        var start = DateTime.Parse(dateFilter.StartDate);
        var end = DateTime.Parse(dateFilter.EndDate);
        start = TimeZoneInfo.ConvertTimeToUtc(start, _timeZone);
        end = TimeZoneInfo.ConvertTimeToUtc(end, _timeZone).AddDays(1);
        IQueryable<DetalleEntrada> query = _entryDetailRepository.Query(de => de.Entrada.FechaRegistro >= start
                                                                              && de.Entrada.FechaRegistro < end)
            .Include(de => de.Entrada)
            .OrderByDescending(ent => ent.Entrada.FechaRegistro);
        return await query.ToListAsync();
    }
}