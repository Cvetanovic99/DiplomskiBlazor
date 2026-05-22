using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface ICompanyResponseDataService
{
    Task<CompanyResponseDto> EditCompanyResponse(EditCompanyResponseDto editCompanyResponseDto);
    Task<CompanyResponseDto> CreateCompanyResponse(CreateCompanyResponseDto  createCompanyResponseDto);
    Task DeleteCompanyResponse(int companyResponseId);
}