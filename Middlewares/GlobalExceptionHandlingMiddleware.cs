using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Farma_api.Middlewares;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            ProblemDetails problem = new()
            {
                Title = "An error occurred",
                Type = "https://httpstatuses.com/500",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message
            };
            var json = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(json);
            context.Response.ContentType = "application/problem+json";
        }
    }
}