using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Cities.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using MediatR;

namespace Diplomski.RatingHub.Web.Data.Services;

public class CityDataService(IMediator mediator) : DataServiceBase(mediator), ICityDataService
{
    public async Task<IPaginatedList<CityDto>> GetCities(string filterValue, QueryArgs queryArgs)
    {
        return await Send(new GetCitiesQuery { FilterValue = filterValue, QueryArgs = queryArgs });
    }
}