using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.UseCases.Notifications.Commands;
using Diplomski.RatingHub.Application.UseCases.Reviews.Commands;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Domain.Models;
using Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserCompaniesPages;
using Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserReviewsPages;
using Diplomski.RatingHub.Web.Components.UserPages.CompanyDetailsPages;
using Diplomski.RatingHub.Web.Data.Interfaces;
using NanoidDotNet;

namespace Diplomski.RatingHub.Web.Data.Services;

public class ReviewDataService : DataServiceBase, IReviewDataService
{
    public ReviewDataService(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    {
        
    }

    public async Task<bool> ValidateReviewAnonymousEditIdentifier(int reviewId, string reviewAnonymousEditIdentifier)
    {
        return await Send(new ValidateReviewAnonymousEditIdentifierQuery
        {
            ReviewId = reviewId, 
            AnonymousEditIdentifier = reviewAnonymousEditIdentifier
        });
    }

    public async Task DeleteReview(int reviewId)
    {
        await Send(new DeleteReviewCommand { ReviewId = reviewId });
    }

    public async Task<IPaginatedList<FilteredReviewDto>> GetFilteredReviews(ReviewsFilterModel filterModel)
    {
        return await Send(new GetFilteredReviewsQuery
        {
            CompanId = filterModel.CompanId,
            FilterValue = filterModel.FilterValue,
            QueryArgs = filterModel.QueryArgs,
            MinOverallScore = filterModel.MinOverallScore,
            OnlyConfirmed = filterModel.OnlyConfirmed,
            OnlyWithCompanyResponse = filterModel.OnlyWithCompanyResponse,
            CurrentAuthenticatedUserId = filterModel.CurrentAuthenticatedUserId
        });
    }

    public async Task<IPaginatedList<FilteredReviewDto>> GetUserCompanyReviews(UserCompanyReviews.UserCompanyReviewsFilterModel filterModel)
    {
        return await Send(new GetUserCompanyReviewsQuery
        {
            CompanyId = filterModel.CompanId,
            FilterValue = filterModel.FilterValue,
            QueryArgs = filterModel.QueryArgs,
            MinOverallScore = filterModel.MinOverallScore,
            OnlyConfirmed = filterModel.OnlyConfirmed,
            OnlyWithoutCompanyResponse = filterModel.OnlyWithoutCompanyResponse
        });
    }

    public async Task<bool> GetIfReviewAlreadyExists(string key, int companyId)
    {
        return await Send(new CheckIfReviewAlreadyExistsQuery { Key = key, CompanyId = companyId });
    }

    public async Task<string?> CreateReview(CreateReviewDto reviewDto)
    {
        string? anonymousEditIdentifier = null;
        if(!reviewDto.IsAuthenticated)
            anonymousEditIdentifier =  await Nanoid.GenerateAsync(Nanoid.Alphabets.LettersAndDigits, 15);

        await Send(new CreateReviewCommand
        {
            Comment = reviewDto.Comment,
            ReviewerFullName = reviewDto.ReviewerFullName,
            IsAnonymousReview = !reviewDto.IsAuthenticated,
            AnonymousEditIdentifier = anonymousEditIdentifier,
            IsCompanyDataTrue = reviewDto.IsCompanyDataTrue,
            ReviewerIdentifier = reviewDto.ReviewerIdentifier,
            CompanyId = reviewDto.CompanyId,
            Images = reviewDto.Images,
            ReviewGrades = reviewDto.ReviewGrades
        });
        
        if (reviewDto.CompanyOwnerId is not null)
        {
            await Send(new CreateNotificationCommand
            {
                Title = "Nova ocena",
                Message = $"Nova recenzija za kompaniju {reviewDto.CompanyName} je objavljena. Možete pogledati ocenu i napisati odgovor.",
                RecipientId = reviewDto.CompanyOwnerId.Value,
                EntityType = nameof(Review)
            });
        }

        return anonymousEditIdentifier;
    }

    public async Task EditReview(EditReviewDto editReviewDto)
    {
        await Send(new EditReviewCommand
        {
            ReviewId = editReviewDto.Id,
            Comment = editReviewDto.Comment,
            ReviewerFullName = editReviewDto.ReviewerFullName,
            IsCompanyDataTrue = editReviewDto.IsCompanyDataTrue,
            Images = editReviewDto.Images.ToList(),
            ReviewGrades = editReviewDto.Grades.ToList()
        });
    }

    public async Task<EditReviewDto> GetReviewForEdit(int reviewId)
    {
        return await Send(new GetReviewForEditQuery { ReviewId = reviewId });
    }

    public async Task LikeOrDislikeReview(int reviewId, int userId)
    {
        await Send(new LikeOrDislikeReviewCommand { ReviewId = reviewId, UserId = userId });
    }

    public async Task<FilteredReviewDto> GetReviewForAdmin(int reviewId)
    {
       return await Send(new GetReviewForAdminQuery { ReviewId = reviewId });
    }

    public async Task<IPaginatedList<FilteredReviewDto>> GetUserReviews(UserReviewsFilterModel filterModel)
    {
        return await Send(new GetUserReviewsQuery
        {
            UserProfileId = filterModel.UserProfileId,
            FilterValue = filterModel.FilterValue,
            QueryArgs = filterModel.QueryArgs,
            MinOverallScore = filterModel.MinOverallScore,
            OnlyConfirmed = filterModel.OnlyConfirmed,
            OnlyWithCompanyResponse = filterModel.OnlyWithCompanyResponse
        });
    }
}