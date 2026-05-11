using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Categories.Queries;

public class GetCategoryParentsQuery : IRequest<IEnumerable<CategoryParentDto>>
{
    public int CategoryId { get; set; }
}

public class GetCategoryParentsQueryValidator : AbstractValidator<GetCategoryParentsQuery>
{
    public GetCategoryParentsQueryValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}

public class GetCategoryParentsQueryHandler : IRequestHandler<GetCategoryParentsQuery, IEnumerable<CategoryParentDto>>
{
    private readonly IMapper _mapper;
    private readonly IDatabaseRepository<Category> _categoryRepository;

    public GetCategoryParentsQueryHandler(IMapper mapper, IDatabaseRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<CategoryParentDto>> Handle(GetCategoryParentsQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetById(request.CategoryId);
        if (category is null)
            throw new ApplicationException("Kategorija ne postoji");
        
        Stack<CategoryParentDto> categories = new Stack<CategoryParentDto>();
        categories.Push(_mapper.Map<CategoryParentDto>(category));
        
                
        int? parentId = category.ParentId;        
        while(parentId.HasValue)
        {
            var parentCategory = await _categoryRepository.GetById(parentId.Value);
            if (parentCategory is null)
                break;
            
            categories.Push(_mapper.Map<CategoryParentDto>(parentCategory));
            parentId = parentCategory.ParentId;
        }


        return categories.ToList();
    }
}
public class CategoryParentDto : IMapFrom<Category>
{
    public int Id { get; set; }
    public string Name { get; set; }
}