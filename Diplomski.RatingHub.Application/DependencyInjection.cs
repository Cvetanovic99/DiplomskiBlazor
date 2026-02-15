using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Diplomski.RatingHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(cfg => { }, assembly);
        return services;
    }
}