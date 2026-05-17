using AutoMapper;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Categories.Commands;

public class CreateCategoryCommand : IRequest<Unit>
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public int SortOrder { get; set; }
    public string? Icon { get; set; }
    public bool ShowOnHomePage { get; set; }
    public int? ParentId  {get; set; }
    public List<CreateCategoryKeywordDto>? Keywords { get; set; } 
    public List<CreateRatingCriterionDto>? RatingCriteria { get; set; }
}

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Slug).NotEmpty();
        RuleFor(x => x.Keywords).Must(KeywordsMustBeValid).WithMessage("Kljucne reci nisu validne");
        RuleFor(x => x.RatingCriteria).Must(RatingCriteriaMustBeValid).WithMessage("Kriterije za ocenjivanje nisu validne");
    }

    private bool KeywordsMustBeValid(List<CreateCategoryKeywordDto>? keywords)
    {
        if (keywords is not null && keywords.Any() && keywords.Exists(x => string.IsNullOrWhiteSpace(x.Keyword)))
            return false;

        return true;
    }

    private bool RatingCriteriaMustBeValid(List<CreateRatingCriterionDto>? ratingCriteria)
    {
        if (ratingCriteria is not null && ratingCriteria.Any() 
                                       && ratingCriteria.All(x => !string.IsNullOrWhiteSpace(x.Name)))
            return true;

        return false;
    }
}

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Unit>
{
    private readonly IDatabaseRepository<Category> _categoryRepository;
    private readonly IMapper _mapper;

    public CreateCategoryCommandHandler(
        IDatabaseRepository<Category> categoryRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var oldCatgory = await _categoryRepository.GetSingleBySpec(
            new Specification<Category>(c => c.Name == request.Name || c.Slug == request.Slug));
        if (oldCatgory is not null)
            throw new AppException("Kategorija sa istim imenom ili slugom već postoji");
        
        var category = new Category
        {
            Name = request.Name,
            Slug = request.Slug,
            SortOrder = request.SortOrder,
            Icon = request.Icon,
            ShowOnHomePage = request.ShowOnHomePage,
        };
        
        if (request.ParentId is not null)
        {
            var parentCategory = await _categoryRepository.GetById(request.ParentId.Value);
            if (parentCategory is null)
                throw new AppException("Roditeljska kategorija ne postoji");
            
            category.Parent = parentCategory;
        }
        
        category.Keywords = _mapper.Map<List<CategoryKeyword>>(request.Keywords);
        category.RatingCriteria = _mapper.Map<List<RatingCriterion>>(request.RatingCriteria);

        await _categoryRepository.Insert(category);
        
        return Unit.Value;
    }
}