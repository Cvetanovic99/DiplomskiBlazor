using Diplomski.RatingHub.Application.UseCases.Companies.Queries;

namespace Diplomski.RatingHub.Application.Interfaces.Repositories;

public interface ICompanyRepository
{
    Task<IEnumerable<T>> GetPopularCompaniesAndProject<T>(int cityId, int categoryId, int take);
}