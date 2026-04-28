using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Web.Models;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface ICategoryDataService
{
    Task<IPaginatedList<CategoryDto>> GetCategories(string filterValue, QueryArgs queryArgs);
    Task CreateCategory(CreateCategoryDto createCategoryDto);
    Task EditCategory(CategoryDto categoryDto);
    Task DeleteCategory(int categoryId);
    Task<IPaginatedList<NewCategorySuggestionDto>> GetNewCategorySuggestions(QueryArgs queryArgs);
    Task EditNewCategorySuggestion(NewCategorySuggestionDto newCategorySuggestionDto);
    Task DeleteCategoryNewSuggestion(int categoryNewSuggestionId);
}