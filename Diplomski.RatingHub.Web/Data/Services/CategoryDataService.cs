using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.UseCases.Categories.Commands;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Web.Components.Shared;
using Diplomski.RatingHub.Web.Data.Interfaces;

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

    public async Task<IPaginatedList<SubcategoryDto>> GetSubcategories(int parentCategoryId, QueryArgs? queryArgs = null)
    {
        return await Send(new GetSubcategoriesQuery
        {
            ParentCategoryId = parentCategoryId, 
            QueryArgs = queryArgs
        });
    }

    public async Task<IEnumerable<CategoryParentDto>> GetCategoryParents(int categoryId)
    {
        return await  Send(new GetCategoryParentsQuery { CategoryId = categoryId });
    }

    public async Task CreateNewCategorySuggestion(SuggestCategoryModel suggestCategoryModel)
    {
        await Send(new CreateNewCategorySuggestionCommand
        {
            CategoryName = suggestCategoryModel.Name,
            Description = suggestCategoryModel.Description,
            ParentCategoryId = suggestCategoryModel.ParentCategoryId
        });
    }

    public async Task<IList<CategoryOrCompanyDto>> GetCategoriesAndCompanies(int cityId, string filterValue, QueryArgs queryArgs)
    {
        return await Send(new GetCategoriesAndCompaniesQuery
        {
            FilterValue = filterValue,
            CityId = cityId,
            QueryArgs = queryArgs
        });
    }

    public async Task<IEnumerable<TopCategoryDto>> GetAllTopCategories()
    {
        return await Send(new GetAllTopCategoriesQuery());
    }

    public async Task<IEnumerable<PopularCategoryDto>> GetPopularCategories()
    {
        return await Send(new GetPopularCategoriesQuery());
    }

    public async Task CreateCategory(CreateCategoryDto createCategoryDto)
    {
        await Send(new CreateCategoryCommand
        {
            Name = createCategoryDto.Name.Trim(),
            Slug = createCategoryDto.Slug.Trim(),
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