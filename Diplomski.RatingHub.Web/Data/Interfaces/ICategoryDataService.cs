using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Web.Models;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface ICategoryDataService
{
    Task<IPaginatedList<CategoryDto>> GetCategories(string filterValue, QueryArgs queryArgs);
    Task<IPaginatedList<object>> GetNewCategorySuggestions(QueryArgs queryArgs);
    Task CreateCategory(CreateCategoryDto createCategoryDto);
}