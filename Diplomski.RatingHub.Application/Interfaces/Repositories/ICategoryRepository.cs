using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Domain.Models;

namespace Diplomski.RatingHub.Application.Interfaces.Repositories;

public interface ICategoryRepository : IDatabaseRepository<Category>
{
    Task<IEnumerable<CategoryWithBreadCrumbDto>> GetCategoriesWithBreadCrumbs(string filterValue, int take);
}