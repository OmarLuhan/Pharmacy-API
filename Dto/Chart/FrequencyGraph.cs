namespace Farma_api.Dto.Chart;

public class FrequencyGraph
{
    public IEnumerable<string> Labels { get; set; } = [];
    public IEnumerable<int> Values { get; set; } = [];
}