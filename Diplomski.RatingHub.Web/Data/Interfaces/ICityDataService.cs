using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Cities.Queries;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface ICityDataService
{
    Task<IPaginatedList<CityDto>> GetCities(string filterValue, QueryArgs queryArgs);
}