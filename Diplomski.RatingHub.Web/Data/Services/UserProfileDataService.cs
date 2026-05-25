using AutoMapper;
using Diplomski.RatingHub.Application.UseCases.UserProfiles.Commands;
using Diplomski.RatingHub.Application.UseCases.UserProfiles.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using MediatR;

namespace Diplomski.RatingHub.Web.Data.Services;

public class UserProfileDataService(IServiceScopeFactory serviceScopeFactory) : DataServiceBase(serviceScopeFactory), IUserProfileDataService
{
    public async Task<UserProfileDto> CreateUserProfile(CreateUserProfileDto createUserProfileDto)
    {
        return await Send(
            new CreateUserProfileCommand
            {
                IdentityUserId = createUserProfileDto.IdentityUserId,
                Name = createUserProfileDto.Name,
                Surname = createUserProfileDto.Surname
            });
    }

    public async Task<CurrentUserProfileDto> GetCurrentUserProfile(string identityId)
    {
        return await Send(new GetCurrentUserProfileQuery { IndetityId = identityId });
    }

    public async Task BlockUserProfile(int userId)
    {
        await Send(new BlockUserProfileCommand { UserId = userId });
    }

    public async Task<UserProfileDto> GetUserProfile(string identityUserId)
    {
        return await Send(new GetUserProfileQuery { IdentityUserId = identityUserId });
    }

    public async Task<UserProfileDto> EditUserProfile(UserProfileDto userProfileDto)
    {
        return await Send(new EditUserProfileCommand
        {
            UserProfileId = userProfileDto.Id,
            Name = userProfileDto.Name,
            Surname = userProfileDto.Surname,
            ProfileImagePath = userProfileDto.ProfileImagePath
        });
    }

    public async Task DeleteUserProfile(int userProfileId)
    {
        throw new NotImplementedException();
    }
}