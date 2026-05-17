namespace Diplomski.RatingHub.Application.Interfaces.Services;

public interface ICurrentUserService
{
    Task SetAuthenticatedUser(string identityId);
    void SetAnonymousUser(string anonymousId);
    CurrentUserDto GetCurrentUserUser();
}

public class CurrentUserDto
{
    public bool IsAuthenticated { get; set; }
    
    public string? AnonymousUserIdentifier { get; set; }
    
    public string? AuthenticatedUserIdentifier { get; set; }
    public int? AuthenticatedUserProfileId { get;  set; }
    public string? AuthenticatedUserProfileName { get; set; }
    public bool? AuthenticatedUserProfileIsBlocked { get; set; }
}