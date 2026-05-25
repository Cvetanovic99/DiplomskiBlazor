using Diplomski.RatingHub.Web.Components.Account.Pages;
using Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.ProfilePages;
using Diplomski.RatingHub.Web.Models;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface IAccountDataService
{
    Task<RegisterUserResult> RegisterUser(RegisterUserDto registerUserDto);
    Task ResendEmailConfirmationLink(string email);
    Task<string> ResendPhoneNumberConfirmationToken(string phoneNumber);
    Task ChangeUserPassword(ChangeUserPasswordDto changeUserPasswordDto);
    Task DeleteUserProfile(string identityUserId, int userProfileId);
}