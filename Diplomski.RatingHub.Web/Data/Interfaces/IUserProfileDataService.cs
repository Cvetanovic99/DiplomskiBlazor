using Diplomski.RatingHub.Web.Models;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface IUserProfileDataService
{ 
    Task CreateUserProfile(CreateUserProfileDto createUserProfileDto);
}