using Diplomski.RatingHub.Application.UseCases.UserProfiles.Queries;

namespace Diplomski.RatingHub.Web.Models;

public class AuthenticatedUserDto
{
    public required string IndetityId { get; set; }
    public required string FullName  { get; set; }
    public int UserProfileId { get; set; }
    public bool IsUserProfileBlocked { get; set; }
}

public class CurrentUserDto
{
    public bool IsAuthenticated { get; set; }
    public required string IndetityId { get; set; }
    
    public CurrentUserProfileDto? CurrentUserProfile { get; set; }
}