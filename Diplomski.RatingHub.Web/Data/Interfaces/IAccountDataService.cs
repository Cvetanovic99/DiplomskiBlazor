using Diplomski.RatingHub.Web.Components.Account.Pages;
using Diplomski.RatingHub.Web.Models;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface IAccountDataService
{
    Task<RegisterUserResult> RegisterUser(RegisterUserDto registerUserDto);
}