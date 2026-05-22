using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserCompaniesPages;
using Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserReviewsPages;
using Diplomski.RatingHub.Web.Components.UserPages.CompanyDetailsPages;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface IReviewDataService
{
    Task<bool> ValidateReviewAnonymousEditIdentifier(int reviewId, string reviewAnonymousEditIdentifier);
    Task DeleteReview(int reviewId);
    Task<IPaginatedList<FilteredReviewDto>> GetFilteredReviews(ReviewsFilterModel filterModel);
    Task<IPaginatedList<FilteredReviewDto>> GetUserCompanyReviews(UserCompanyReviews.UserCompanyReviewsFilterModel filterModel);
    Task<bool> GetIfReviewAlreadyExists(string key, int companyId);
    Task<string?> CreateReview(CreateReviewDto reviewDto);
    Task EditReview(EditReviewDto editReviewDto);
    Task<EditReviewDto> GetReviewForEdit(int reviewId);
    Task LikeOrDislikeReview(int reviewId, int userId);
    Task<FilteredReviewDto> GetReviewForAdmin(int reviewId);
    Task<IPaginatedList<FilteredReviewDto>> GetUserReviews(UserReviewsFilterModel filterModel);
}