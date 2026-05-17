using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Web.Components.UserPages.CompanyDetailsPages;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface IReviewDataService
{
    Task<bool> ValidateReviewAnonymousEditIdentifier(int reviewId, string reviewAnonymousEditIdentifier);
    Task DeleteReview(int reviewId);
    Task<IPaginatedList<FilteredReviewDto>> GetFilteredReviews(ReviewsFilterModel filterModel);
    Task<bool> GetIfReviewAlreadyExists(string key, int companyId);
}