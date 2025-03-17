namespace Farma_api.Dependencies;
public static class Cors
{
    public static void InjectCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            options.AddPolicy("AllowSpecificOrigin",
                builder =>
                {
                    builder.WithOrigins("https://crisfarma.net.pe");
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
        });
    }
}