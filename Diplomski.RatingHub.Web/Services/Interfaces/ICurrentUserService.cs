using Diplomski.RatingHub.Web.Models;
using Microsoft.JSInterop;

namespace Diplomski.RatingHub.Web.Services.Interfaces;

public interface ICurrentUserService
{
    Task<AuthenticatedUserDto?>  GetAuthenticatedUserAsync();
    Task<CurrentUserDto?>  GetCurrentUserAsync(IJSRuntime jsRuntime);
}