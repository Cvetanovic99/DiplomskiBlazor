using Diplomski.RatingHub.Infrastructure.Auth.Models;
using Diplomski.RatingHub.Infrastructure.Notifications.Email;
using Diplomski.RatingHub.Infrastructure.Persistence.Contexts;
using Diplomski.RatingHub.Web.Components.Account;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Data.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Diplomski.RatingHub.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddScoped<ICityDataService, CityDataService>();
        services.AddScoped<IAccountDataService, AccountDataService>();
        services.AddScoped<IUserProfileDataService, UserProfileDataService>();
        
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        AddAuthenticationSupport(services);
        
        services.AddScoped(typeof(IEmailSender<>), typeof(IdentityEmailSender<>));
        services.AddScoped<IEmailSender<ApplicationUser>, IdentityEmailSender<ApplicationUser>>();
        
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
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();
    }
}