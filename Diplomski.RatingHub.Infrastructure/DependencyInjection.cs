using Diplomski.RatingHub.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Diplomski.RatingHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

        switch (environment)
        {
            case "Development":
                AddLocalDatabase(services, configuration);
                break;
            default:
                throw new Exception("Environment variable is not correctly added.");
        }
        
        return services;
    }
    
    public static void AddLocalDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
    }
}