using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;

namespace Diplomski.RatingHub.Web.Data.Services;

public class CompanyDataService(IServiceScopeFactory serviceScopeFactory) : DataServiceBase(serviceScopeFactory), ICompanyDataService
{
    public async Task<IPaginatedList<CompanyDto>> GetCompanies(string filterValue, int cityId, QueryArgs queryArgs)
    {
        return await Send(new GetCompaniesQuery
        {
            FilterValue = filterValue,
            CityId = cityId,
            QueryArgs = queryArgs
        });
    }
}