using System.Globalization;
using Farma_api.Dto.Chart;
using Farma_api.Models;
using Farma_api.Repository;
using Microsoft.EntityFrameworkCore;

namespace Farma_api.Services;

public interface IChartsService
{
    Task<LinesChart> RevenuesChart();
    Task<BarChart> NewSalesChart();
    Task<FrequencyGraph> FrequencyGraph(); // products to be finished
    Task<ProductZero> ProductsZero();
    Task<DistributionGraph> DistributionGraph(); // sales distribution
}

public class ChartsService(
    ISaleRepository saleRepository,
    IProductRepository productRepository,
    IGenericRepository<DetalleVenta> detailRepository)
    : IChartsService
{
    private readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

    public async Task<BarChart> NewSalesChart()
    {
        var utcNow = DateTime.UtcNow.Date;
        IEnumerable<Venta> sales =
            await saleRepository.Query(s => s.FechaRegistro >= utcNow.AddDays(-6).Date).ToListAsync();
        var barChart = new BarChart
        {
            Data = sales.Select(s => new
            {
                Fecha = TimeZoneInfo.ConvertTimeFromUtc(s.FechaRegistro, _timeZone).Date
            }).GroupBy(s => s.Fecha)
                .Select(g => new DataPoint { X = g.Key.ToString("dd MMM"), Y = g.Count() })
                .ToList(),
            CountSales = sales.Count()
        };
        return barChart;
    }

    public async Task<FrequencyGraph> FrequencyGraph()
    {
        var products = await productRepository.Query()
            .Where(l => l.Lotes.Sum(batch => batch.StockLote) > 0)
            .Include(p => p.Lotes)
            .Select(p => new
            {
                p.Nombre,
                StockTotal = p.Lotes.Sum(l => l.StockLote)
            })
            .OrderBy(p => p.StockTotal)
            .Take(12)
            .ToListAsync();
        var frequencyGraph = new FrequencyGraph
        {
            Labels = products.Select(p => p.Nombre),
            Values = products.Select(p => p.StockTotal)
        };

        return frequencyGraph;
    }

    public async Task<ProductZero> ProductsZero()
    {
        var products = await productRepository.Query().Where(p => p.Lotes.Sum(l => l.StockLote) <= 0)
            .Select(p => new
            {
                p.Nombre,
                StockTotal = p.Lotes.Sum(l => l.StockLote)
            }).ToListAsync();
        var productsZero = new ProductZero
        {
            Name = products.Select(p => p.Nombre),
            Quantity = products.Select(p => p.StockTotal)
        };
        return productsZero;
    }

    public async Task<DistributionGraph> DistributionGraph()
    {
        var lastWeek = DateTime.UtcNow.AddDays(-6).Date;
        var result = await detailRepository
            .Query()
            .Where(d => d.Venta.FechaRegistro >= lastWeek)
            .GroupBy(d => d.ProductoId)
            .Select(g => new
            {
                Name = g.First().NombreProducto,
                QuantitySum = g.Sum(d => d.Cantidad)
            }).OrderByDescending(p => p.QuantitySum).Take(5).ToListAsync();
        var distributionGraph = new DistributionGraph
        {
            Labels = result.Select(p => p.Name),
            Values = result.Select(p => p.QuantitySum)
        };
        return distributionGraph;
    }

    public async Task<LinesChart> RevenuesChart()
    {
        var lineChart = new LinesChart();
        var utcNow = DateTime.UtcNow;
        var firstDay = GetDayOfWeek(utcNow);
        var lastDay = GetDayOfWeek(utcNow, false);
        var peruNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _timeZone);
        var saFirstDay = GetDayOfWeek(peruNow);
        var saLastDay = GetDayOfWeek(peruNow, false);
        lineChart.Revenue = await GetRevenues(firstDay, lastDay, saFirstDay, saLastDay);
        lineChart.PreviousRevenue = await GetRevenues(firstDay.AddDays(-7), lastDay.AddDays(-7), saFirstDay.AddDays(-7),
            saLastDay.AddDays(-7));
        lineChart.Days = ["domingo", "lunes", "martes", "miercoles", "jueves", "viernes", "sabado"];
        lineChart.Period = saFirstDay.ToString("dd MMM yy") + " - " + saLastDay.ToString("dd MMM yy");
        lineChart.SumRevenue = lineChart.Revenue.Sum();
        lineChart.SumPreviousRevenue = lineChart.PreviousRevenue.Sum();
        lineChart.PercentageGrowth = lineChart.SumPreviousRevenue == 0
            ? 100
            : ((lineChart.SumRevenue - lineChart.SumPreviousRevenue) / lineChart.SumPreviousRevenue * 100);
        return lineChart;
    }

    private async Task<IEnumerable<decimal>> GetRevenues(DateTime startUtcDate, DateTime endUtcDate,
        DateTime startSaDate, DateTime endSaDate)
    {
        endUtcDate = endUtcDate.AddDays(1);
        var sales = await saleRepository
            .Query(s => s.FechaRegistro >= startUtcDate.Date && s.FechaRegistro < endUtcDate.Date && s.Estado != false)
            .ToListAsync();
        var dailyRevenues = sales
            .Select(s => new
            {
                Fecha = TimeZoneInfo.ConvertTimeFromUtc(s.FechaRegistro, _timeZone).Date,
                s.Total
            }).GroupBy(s => s.Fecha)
            .Select(g => new
            {
                Fecha = g.Key,
                TotalPago = g.Sum(s => s.Total)
            })
            .ToDictionary(g => g.Fecha, g => g.TotalPago);
        var revenues = new List<decimal>();
        for (var date = startSaDate.Date; date <= endSaDate; date = date.Date.AddDays(1))
            revenues.Add(dailyRevenues.GetValueOrDefault(date, 0));
        return revenues;
    }

    private static DateTime GetDayOfWeek(DateTime date, bool isFirstDay = true)
    {
        var firstDayOfWeek = CultureInfo.InvariantCulture.DateTimeFormat.FirstDayOfWeek;
        var offset = date.DayOfWeek - firstDayOfWeek;
        if (offset < 0)
            offset += 7;
        return isFirstDay ? date.AddDays(-offset) : date.AddDays(6 - offset);
    }
}