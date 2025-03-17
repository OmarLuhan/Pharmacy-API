using System.Net;

namespace Farma_api.Helpers;

public class ChartResponse<T>
{
    public HttpStatusCode Status { set; get; }
    public bool IsSuccess { set; get; }
    public List<string> ErrorMessage { set; get; } = [];
    public T? Data { set; get; }
    public string PercentageGrowth { set; get; } = null!;
    public float Value { set; get; }
    public float PreviousValue { set; get; }
    public string Period { set; get; } = "";
}

public class EmptyResponse
{
    public HttpStatusCode Status { set; get; }
    public bool IsSuccess { set; get; }
    public List<string> ErrorMessage { set; get; } = [];
}

public class Response<T>
{
    public HttpStatusCode Status { set; get; }
    public bool IsSuccess { set; get; }
    public List<string> ErrorMessage { set; get; } = [];
    public T? Data { set; get; }
    public int TotalPages { set; get; }
}

public class BasicResponse<T>
{
    public HttpStatusCode Status { get; set; }
    public bool IsSuccess { get; set; }
    public List<string> ErrorMessage { get; set; } = [];
    public T? Data { get; set; }
}