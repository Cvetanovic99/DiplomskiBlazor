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
}