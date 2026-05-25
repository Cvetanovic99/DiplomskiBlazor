using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Domain.Models;

namespace Diplomski.RatingHub.Application.UseCases.UserProfiles.Queries;

public class GetUserProfilesQuery
{
    
}

public class UserProfileDto : IMapFrom<UserProfile>
{
    public int Id { get; set; }
    public string IdentityUserId { get; set; }
    public string Name { get; set; } 
    public string Surname { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}