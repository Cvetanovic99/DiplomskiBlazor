using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface ICompanyDataService
{
    Task<IPaginatedList<CompanyDto>> GetCompanies(string filterValue, int cityId, QueryArgs queryArgs);
}