using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Application.UseCases.CompanyVerifications.Queries;
using Diplomski.RatingHub.Domain.Enums;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface ICompanyVerificationRequestDataService
{
    Task<IPaginatedList<CompanyVerificationRequestDto>> GetVerificationRequests(string search, CompanyVerificationRequestStatus? status, QueryArgs args);
    Task DeleteVerificationRequest(int requestId);
    Task SendCompanyVerificationRulesToUser(string userEmail, int userId, string identifier, string companyName);
    Task VerifyCompany(int companyId, int recipiendId, string companyName);
    Task EditVerificationRequestStatus(int requestId, CompanyVerificationRequestStatus newStatus);
    Task<UserCompanyVerificationRequestDto> CreateVerificationRequestStatus(int userProfileId, int CompanyId, string contactEmail, string? description);
}