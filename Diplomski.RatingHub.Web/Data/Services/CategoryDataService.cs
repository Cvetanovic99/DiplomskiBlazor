using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Categories.Commands;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Domain.Models;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using MediatR;

namespace Diplomski.RatingHub.Web.Data.Services;

public class CategoryDataService(IMediator mediator) : DataServiceBase(mediator), ICategoryDataService
{
    public async Task<IPaginatedList<CategoryDto>> GetCategories(string filterValue, QueryArgs queryArgs)
    {
        return await Send(new GetCategoriesQuery { FilterValue = filterValue, QueryArgs = queryArgs });
    }

    public Task<IPaginatedList<object>> GetNewCategorySuggestions(QueryArgs queryArgs)
    {
        throw new NotImplementedException();
    }

    public async Task CreateCategory(CreateCategoryDto createCategoryDto)
    {
        await Send(new CreateCategoryCommand
        {
            Name = createCategoryDto.Name,
            Slug = createCategoryDto.Slug,
            SortOrder = createCategoryDto.SortOrder,
            ParentId = createCategoryDto.ParentId,
            Keywords = createCategoryDto.Keywords,
            RatingCriteria = createCategoryDto.RatingCriteria
        });
    }
}