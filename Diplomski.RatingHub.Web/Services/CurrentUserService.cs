using Diplomski.RatingHub.Application.Interfaces.Services;
using Diplomski.RatingHub.Web.Data.Interfaces;

namespace Diplomski.RatingHub.Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IUserProfileDataService  _userProfileDataService;
    
    public bool IsAuthenticated { get; private set; }
    
    public string? AnonymousUserIdentifier { get; private set; }
    
    public string? AuthenticatedUserIdentifier { get; private set; }
    public int? AuthenticatedUserProfileId { get; private set; }
    public string? AuthenticatedUserProfileName { get; private set; }
    public bool? AuthenticatedUserProfileIsBlocked { get; private set; }

    public CurrentUserService(IUserProfileDataService userProfileDataService)
    {
        _userProfileDataService = userProfileDataService;
    }

    public async Task SetAuthenticatedUser(string identityId)
    {
        IsAuthenticated = true;
        AuthenticatedUserIdentifier =  identityId;
        try
        {
            var userProfile = await _userProfileDataService.GetCurrentUserProfile(identityId);
            AuthenticatedUserProfileId = userProfile.Id;
            AuthenticatedUserProfileName = userProfile.FullName;
            AuthenticatedUserProfileIsBlocked = userProfile.Blocked;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void SetAnonymousUser(string anonymousId)
    {
        IsAuthenticated = false;
        AnonymousUserIdentifier =  anonymousId;
    }

    public CurrentUserDto GetCurrentUserUser()
    {
        return new CurrentUserDto
        { 
            IsAuthenticated = IsAuthenticated,
    
            AnonymousUserIdentifier = AnonymousUserIdentifier,
    
            AuthenticatedUserIdentifier = AuthenticatedUserIdentifier,
            AuthenticatedUserProfileId =  AuthenticatedUserProfileId, 
            AuthenticatedUserProfileName = AuthenticatedUserProfileName,
            AuthenticatedUserProfileIsBlocked = AuthenticatedUserProfileIsBlocked
        };
    }
}