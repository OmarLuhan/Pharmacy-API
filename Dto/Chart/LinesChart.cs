using SixLabors.ImageSharp;

namespace Farma_api.Dto.Chart;

public class LinesChart
{
    public IEnumerable<decimal> Revenue { get; set; } = [];
    public IEnumerable<decimal> PreviousRevenue { get; set; } = [];
    public IEnumerable<string> Days { get; set; } = [];
    public decimal SumRevenue { get; set; }
    public decimal SumPreviousRevenue { get; set; }
    public string Period { get; set; } = null!;
    public decimal PercentageGrowth { get; set; }
}