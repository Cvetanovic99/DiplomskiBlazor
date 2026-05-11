using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Cities.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using MediatR;

namespace Diplomski.RatingHub.Web.Data.Services;

public class CityDataService(IServiceScopeFactory serviceScopeFactory) : DataServiceBase(serviceScopeFactory), ICityDataService
{
    public async Task<IPaginatedList<CityDto>> GetCities(string filterValue, QueryArgs queryArgs)
    {
        return await Send(new GetCitiesQuery { FilterValue = filterValue, QueryArgs = queryArgs });
    }

    public async Task<CityDto> GetCityById(int cityId)
    {
        return await Send(new GetCityByIdQuery{ CityId = cityId });
    }
}