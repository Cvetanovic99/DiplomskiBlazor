using Diplomski.RatingHub.Application.Interfaces.Services;
using Diplomski.RatingHub.Infrastructure.Auth.Models;
using Diplomski.RatingHub.Infrastructure.Notifications.Email;
using Diplomski.RatingHub.Infrastructure.Persistence.Contexts;
using Diplomski.RatingHub.Web.Components.Account;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Data.Services;
using Diplomski.RatingHub.Web.Services;
using Diplomski.RatingHub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Diplomski.RatingHub.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICityDataService, CityDataService>();
        services.AddScoped<IAccountDataService, AccountDataService>();
        services.AddScoped<IUserProfileDataService, UserProfileDataService>();
        services.AddScoped<ICategoryDataService, CategoryDataService>();
        services.AddScoped<IReportedContentDataService, ReportedContentDataService>();
        services.AddScoped<ICompanyVerificationRequestDataService, CompanyVerificationRequestDataService>();
        services.AddScoped<ICompanyDataService, CompanyDataService>();
        services.AddScoped<IReviewDataService, ReviewDataService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        AddAuthenticationSupport(services);
        
        services.AddScoped<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
        
        AddHttpClient(services, configuration);
        
        return services;
    }

    private static void AddAuthenticationSupport(IServiceCollection services)
    {
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();
        
        services.AddDatabaseDeveloperPageExceptionFilter();
        
        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true; 
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();
    }

    private static void AddHttpClient(IServiceCollection services, IConfiguration configuration)
    {
        var apiUrl = configuration.GetConnectionString("ApiUrl") ??
                               throw new InvalidOperationException("Connection string 'ApiUrl' not found.");

        services
            .AddHttpClient<IHttpService, HttpService>(client => client.BaseAddress = new Uri(apiUrl));
    }
}