using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.UseCases.Reviews.Commands;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Web.Components.UserPages.CompanyDetailsPages;
using Diplomski.RatingHub.Web.Data.Interfaces;

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
            OnlyWithCompanyResponse = filterModel.OnlyWithCompanyResponse
        });
    }

    public async Task<bool> GetIfReviewAlreadyExists(string key, int companyId)
    {
        return await Send(new CheckIfReviewAlreadyExistsQuery { Key = key, CompanyId = companyId });
    }
}