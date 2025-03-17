namespace Farma_api.Dto.Chart;

public class DataPoint
{
    public string X { get; set; } = "";
    public int Y { get; set; }
}

public class BarChart
{
    public List<DataPoint> Data { get; set; } = [];
    public int CountSales { get; set; }
}