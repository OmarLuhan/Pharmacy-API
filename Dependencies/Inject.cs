using Farma_api.Helpers;
using Farma_api.Models;
using Farma_api.Repository;
using Farma_api.Services;
using Microsoft.EntityFrameworkCore;

namespace Farma_api.Dependencies;

public static class Injects
{
    public static void Inject(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FarmadbContext>(options =>
            options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 30))));
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IGoodsReceivedRepository, GoodsReceivedRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        //services.AddTransient<GlobalExceptionHandlingMiddleware>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFirebaseService, FirebaseService>();
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IBatchService, BatchService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IGoodsReceivedService, GoodsReceivedService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IChartsService, ChartsService>();
    }
}