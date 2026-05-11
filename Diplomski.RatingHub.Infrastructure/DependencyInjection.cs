using System.Net.Http.Headers;
using Diplomski.RatingHub.Application.Interfaces.Notifications;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Interfaces.Storage;
using Diplomski.RatingHub.Infrastructure.Auth.Models;
using Diplomski.RatingHub.Infrastructure.Auth.Stores;
using Diplomski.RatingHub.Infrastructure.Notifications.Email;
using Diplomski.RatingHub.Infrastructure.Notifications.Email.Models;
using Diplomski.RatingHub.Infrastructure.Notifications.Sms;
using Diplomski.RatingHub.Infrastructure.Notifications.Sms.Models;
using Diplomski.RatingHub.Infrastructure.Persistence.Contexts;
using Diplomski.RatingHub.Infrastructure.Persistence.Repositories;
using Diplomski.RatingHub.Infrastructure.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        
        services.AddScoped(typeof(IDatabaseRepository<>), typeof(DatabaseRepository<>));
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IUserConfirmation<ApplicationUser>, UserConfirmationStore>();
        services.AddScoped<IFileService, FileService>();
        AddEmailNotification(services, configuration);
        AddSmsNotification(services, configuration);
        
        return services;
    }
    
    private static void AddLocalDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
    }

    private static void AddEmailNotification(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BrevoOptions>(configuration.GetSection(BrevoOptions.SectionName));
        
        services.AddHttpClient<IBrevoClient, BrevoClient>((sp, http) =>
        {
            var opt = sp.GetRequiredService<IOptions<BrevoOptions>>().Value;

            http.BaseAddress = new Uri(opt.BaseUrl);
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            http.DefaultRequestHeaders.Add("api-key", opt.ApiKey);
        });
        
        services.AddScoped<IEmailNotificationService, EmailNotificationService>();
        //services.AddScoped(typeof(IEmailSender<>), typeof(IdentityEmailSender<>));
    }

    private static void AddSmsNotification(IServiceCollection services, IConfiguration configuration)
    {
        //This is another way of registering options
        services.AddOptions<InfobipOptions>()
            .Bind(configuration.GetSection(InfobipOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.BaseUrl), "Infobip BaseUrl is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "Infobip ApiKey is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Sender), "Infobip Sender is required")
            .ValidateOnStart();

        services.AddHttpClient<IInfobipClient, InfobipClient>((sp, http) =>
        {
            var opt = sp.GetRequiredService<IOptions<InfobipOptions>>().Value;

            http.BaseAddress = new Uri(opt.BaseUrl);
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("App", opt.ApiKey);
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        });

        services.AddScoped<ISmsNotificationService, SmsNotificationService>();
    }
}