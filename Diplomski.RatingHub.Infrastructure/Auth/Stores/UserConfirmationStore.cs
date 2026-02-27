using Diplomski.RatingHub.Infrastructure.Auth.Enums;
using Diplomski.RatingHub.Infrastructure.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace Diplomski.RatingHub.Infrastructure.Auth.Stores;

public sealed class  UserConfirmationStore : IUserConfirmation<ApplicationUser>
{
    public Task<bool> IsConfirmedAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        return user.RegistrationMethod switch
        {
            RegistrationMethod.Email => Task.FromResult(user.EmailConfirmed),
            RegistrationMethod.Phone => Task.FromResult(user.PhoneNumberConfirmed),
            _ => Task.FromResult(false)
        };
    }
}