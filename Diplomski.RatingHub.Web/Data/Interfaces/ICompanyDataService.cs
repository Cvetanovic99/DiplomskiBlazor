using Diplomski.RatingHub.Application.Enums;
using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Web.Models;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface ICompanyDataService
{
    Task<IPaginatedList<CompanyDto>> GetCompanies(string filterValue, int cityId, QueryArgs queryArgs);
    Task<CreateCompanyAsAnonymousResponse> CreateCompanyAsAnonymous(CreateCompanyDto  createCompanyDto);
    Task<int> CreateCompanyAsOwner(CreateCompanyDto  createCompanyDto);
    Task<IPaginatedList<FilteredCompanyDto>> GetFilteredCompanies(int cityId, int categoryId, string filterValue, 
        double overallRatingGrade, QueryArgs queryArgs, CompanyClaimStatusFilterOptions claimStatus, CompanyVerificationStatusFilterOptions verificationStatus);
    Task<IEnumerable<PopularCompanyDto>> GetPopularCompanies(int cityId, int categoryId, int take);
}