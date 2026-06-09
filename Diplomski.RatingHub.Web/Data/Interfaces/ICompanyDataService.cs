using Diplomski.RatingHub.Application.Enums;
using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface ICompanyDataService
{
    Task<IPaginatedList<CompanyDto>> GetCompanies(string filterValue, int cityId, QueryArgs queryArgs);
    Task<CreateCompanyAsAnonymousResponse> CreateCompanyAsAnonymous(CreateCompanyDto  createCompanyDto);
    Task<int> CreateCompanyAsOwner(CreateCompanyDto  createCompanyDto);
    Task EditCompany(EditCompanyDto  editCompanyDto);
    Task<IPaginatedList<FilteredCompanyDto>> GetFilteredCompanies(int cityId, int categoryId, string filterValue, 
        double overallRatingGrade, QueryArgs queryArgs, CompanyClaimStatusFilterOptions claimStatus, CompanyVerificationStatusFilterOptions verificationStatus, string orderBy);
    Task<IEnumerable<PopularCompanyDto>> GetPopularCompanies(int cityId, int categoryId, int take);
    Task<CompanyDetailsDto> GetCompanyDetails(int companyId);
    Task<CompanyDetailsAdditionalDataDto> GetCompanyDetailsAdditionalData(int companyId);
    Task<bool> ValidateCompanyAnonymousEditIdentifier(int companyId, string companyAnonymousEditIdentifier);
    Task DeleteCompanyAsAnonymous(int companyId, bool isAdminDeleting = false);
    Task DeleteCompanyAsOwner(int companyId);
    Task<EditCompanyDto> GetCompanyForEdit(int companyId);
    Task<CompanyWithRatingCriteriaDto> GetCompanyWithRatingCriteria(int companyId);
    Task<IPaginatedList<UserCompanyDto>> GetUserCompanies(int userProfileId, QueryArgs queryArgs);
    Task SetCompanyOwner(int userProfileId, string claimCompanyIdentifier);
    Task SetCompanyAsSponsored(int companyId);
    Task<string> CreateCheckoutSession(int companyId);
    Task RemoveCompanyFromSponsored(int companyId);
}