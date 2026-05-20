using Diplomski.RatingHub.Application.UseCases.UserProfiles.Commands;
using Diplomski.RatingHub.Application.UseCases.UserProfiles.Queries;
using Diplomski.RatingHub.Web.Models;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface IUserProfileDataService
{ 
    Task<UserProfileDto> CreateUserProfile(CreateUserProfileDto createUserProfileDto);
    Task<CurrentUserProfileDto> GetCurrentUserProfile(string identityId);
    Task BlockUserProfile(int userId);
    Task<bool> CheckForNewNotifications(int userId);
}