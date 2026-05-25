using System.Security.Claims;
using Diplomski.RatingHub.Web.Constants;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using Diplomski.RatingHub.Web.Services.Interfaces;
using Diplomski.RatingHub.Web.Utilities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace Diplomski.RatingHub.Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IUserProfileDataService  _userProfileDataService;
    private readonly AuthenticationStateProvider _authStateProvider;

    public CurrentUserService(
        IUserProfileDataService userProfileDataService,
        AuthenticationStateProvider authStateProvider)
    {
        _userProfileDataService = userProfileDataService;
        _authStateProvider = authStateProvider;
    }
    

    public async Task<AuthenticatedUserDto?> GetAuthenticatedUserAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        
        if (user.Identity?.IsAuthenticated == true)
        {
            string identityId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(identityId))
            {
                try
                {
                    var userProfile = await _userProfileDataService.GetCurrentUserProfile(identityId);

                    return new AuthenticatedUserDto
                    {
                        IdentityId = identityId,
                        FullName = userProfile.FullName,
                        UserProfileId = userProfile.Id,
                        IsUserProfileBlocked = userProfile.Blocked
                    };
                }
                catch 
                {
                    return null;
                }
            }
        }
        
        return null;
    }

    public async Task<CurrentUserDto?> GetCurrentUserAsync(IJSRuntime jsRuntime)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        
        if (user.Identity?.IsAuthenticated == true)
        {
            string identityId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(identityId))
            {
                try
                {
                    var userProfile = await _userProfileDataService.GetCurrentUserProfile(identityId);

                    return new CurrentUserDto
                    {
                        IsAuthenticated = true,
                        IndetityId = identityId,
                        CurrentUserProfile = userProfile
                    };
                }
                catch 
                {
                    return null;
                }
            }
            
            return null;
        }
        else
        {
            string? customGuid = await jsRuntime.GetItemFromLocalStorage(LocalStorageKeys.AnonymousUserCustomGuidKey);
            if (string.IsNullOrEmpty(customGuid))
            {
                string guid = Guid.NewGuid().ToString();
                bool res = await jsRuntime.SetItemToLocalStorage(LocalStorageKeys.AnonymousUserCustomGuidKey, guid);
                if (!res)
                {
                    return null;
                }
                
                return new CurrentUserDto
                {
                    IsAuthenticated = false,
                    IndetityId = guid,
                    CurrentUserProfile = null
                };
            }
            
            return new CurrentUserDto
            {
                IsAuthenticated = false,
                IndetityId = customGuid,
                CurrentUserProfile = null
            };
        }
    }
}