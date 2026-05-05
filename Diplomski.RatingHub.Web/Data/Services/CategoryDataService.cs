using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Categories.Commands;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Domain.Models;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using MediatR;

namespace Diplomski.RatingHub.Web.Data.Services;

public class CategoryDataService(IServiceScopeFactory serviceScopeFactory) : DataServiceBase(serviceScopeFactory), ICategoryDataService
{
    public async Task<IPaginatedList<CategoryDto>> GetCategories(string filterValue, QueryArgs queryArgs)
    {
        return await Send(new GetCategoriesQuery { FilterValue = filterValue, QueryArgs = queryArgs });
    }

    public async Task<IEnumerable<CategoryWithBreadCrumbDto>> GetCategoriesWithBreadCrumb(string filterValue, int take)
    {
        return await Send(new GetCategoriesWithBreadCrumbQuery { FilterValue = filterValue, Take = take });
    }

    public async Task<IPaginatedList<NewCategorySuggestionDto>> GetNewCategorySuggestions(QueryArgs queryArgs)
    {
        return await Send(new GetNewCategorySuggestionsQuery { QueryArgs = queryArgs });
    }

    public async Task EditNewCategorySuggestion(NewCategorySuggestionDto newCategorySuggestionDto)
    {
        await Send(new EditNewCategorySuggestionCommand
        {
            NewCategorySuggestionId = newCategorySuggestionDto.Id,
            Status = newCategorySuggestionDto.Status
        });
    }

    public async Task DeleteCategoryNewSuggestion(int categoryNewSuggestionId)
    {
        await Send(new DeleteNewCategorySuggestionCommand { NewCategorySuggestionId = categoryNewSuggestionId });
    }

    public async Task CreateCategory(CreateCategoryDto createCategoryDto)
    {
        await Send(new CreateCategoryCommand
        {
            Name = createCategoryDto.Name,
            Slug = createCategoryDto.Slug,
            SortOrder = createCategoryDto.SortOrder,
            Icon = createCategoryDto.Icon,
            ShowOnHomePage = createCategoryDto.ShowOnHomePage,
            ParentId = createCategoryDto.ParentId,
            Keywords = createCategoryDto.Keywords,
            RatingCriteria = createCategoryDto.RatingCriteria
        });
    }
    
    public async Task EditCategory(CategoryDto categoryDto)
    {
        await Send(new EditCategoryCommand
        {
            CategoryId = categoryDto.Id,
            Name = categoryDto.Name,
            Slug = categoryDto.Slug,
            SortOrder = categoryDto.SortOrder,
            Icon = categoryDto.Icon,
            ShowOnHomePage = categoryDto.ShowOnHomePage,
            Keywords = categoryDto.Keywords,
            RatingCriteria = categoryDto.RatingCriteria
        });
    }

    public async Task DeleteCategory(int categoryId)
    {
        await Send(new DeleteCategoryCommand { CategoryId = categoryId });
    }
}