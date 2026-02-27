using Diplomski.RatingHub.Infrastructure.Auth.Enums;
using Microsoft.AspNetCore.Identity;

namespace Diplomski.RatingHub.Infrastructure.Auth.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public required RegistrationMethod RegistrationMethod { get; set; }
}