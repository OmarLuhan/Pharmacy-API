namespace Farma_api.Dto.Chart;

public class DistributionGraph
{
    public IEnumerable<string> Labels { get; set; } = null!;
    public IEnumerable<int> Values { get; set; } = null!;
}