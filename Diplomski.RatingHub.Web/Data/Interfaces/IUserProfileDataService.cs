using Diplomski.RatingHub.Application.UseCases.UserProfiles.Commands;
using Diplomski.RatingHub.Web.Models;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface IUserProfileDataService
{ 
    Task<UserProfileDto> CreateUserProfile(CreateUserProfileDto createUserProfileDto);
}