using AutoMapper;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Categories.Commands;

public class EditCategoryCommand : IRequest<Unit>
{
    public int CategoryId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public int SortOrder { get; set; }
    public string? Icon { get; set; }
    public bool ShowOnHomePage { get; set; }
    public List<CategoryKeywordDto> Keywords { get; set; } = new();
    public List<RatingCriterionDto> RatingCriteria { get; set; } = new();
}

public class EditCategoryCommandValidator : AbstractValidator<EditCategoryCommand>
{
    public EditCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Slug).NotEmpty();
        RuleFor(x => x.Keywords).Must(KeywordsMustBeValid).WithMessage("Kljucne reci nisu validne");
        RuleFor(x => x.RatingCriteria).Must(RatingCriteriaMustBeValid).WithMessage("Kriterije za ocenjivanje nisu validne");
    }

    private bool KeywordsMustBeValid(List<CategoryKeywordDto> keywords)
    {
        if (keywords.Any() && keywords.Exists(x => string.IsNullOrWhiteSpace(x.Keyword)))
            return false;

        return true;
    }

    private bool RatingCriteriaMustBeValid(List<RatingCriterionDto> ratingCriteria)
    {
        if (ratingCriteria.Any() && ratingCriteria.All(x => !string.IsNullOrWhiteSpace(x.Name)))
            return true;

        return false;
    }
}

public class EditCategoryCommandHandler : IRequestHandler<EditCategoryCommand, Unit>
{
    private readonly IDatabaseRepository<Category> _categoryRepository;
    private readonly IDatabaseRepository<CategoryKeyword> _categoryKeywordRepository;
    private readonly IMapper _mapper;

    public EditCategoryCommandHandler(
        IDatabaseRepository<Category> categoryRepository,
        IDatabaseRepository<CategoryKeyword> categoryKeywordRepository,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _categoryKeywordRepository = categoryKeywordRepository;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(EditCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetSingleBySpec(new Specification<Category>(
            c => c.Id == request.CategoryId)
            .AddInclude(c => c.Keywords)
            .AddInclude("RatingCriteria.ReviewGrades"));
        if (category is null)
            throw new AppException("Kategorija ne postoji");

        
        category.Keywords.Clear();
        var newKeywords = _mapper.Map<List<CategoryKeyword>>(request.Keywords);
        foreach (var keyword in newKeywords)
        {
            category.Keywords.Add(keyword);
        }

        var existingCriteria = category.RatingCriteria.ToList();
        var incomingCriteria = request.RatingCriteria;
        
        foreach (var incoming in incomingCriteria)
        {
            if (incoming.Id.HasValue)
            {
                var existing = existingCriteria.FirstOrDefault(x => x.Id == incoming.Id.Value);

                if (existing is null)
                    throw new AppException($"Kriterijum sa ID {incoming.Id.Value} ne postoji.");

                existing.Name = incoming.Name;
                existing.SortOrder = incoming.SortOrder;
                existing.IsActive = incoming.IsActive;
            }
            else
            {
                category.RatingCriteria.Add(new RatingCriterion
                {
                    Name = incoming.Name,
                    SortOrder = incoming.SortOrder,
                    IsActive = incoming.IsActive
                });
            }
        }
        
        var incomingIds = incomingCriteria
            .Where(x => x.Id.HasValue)
            .Select(x => x.Id!.Value)
            .ToHashSet();

        var removedCriteria = existingCriteria
            .Where(x => !incomingIds.Contains(x.Id))
            .ToList();

        foreach (var removed in removedCriteria)
        {
            if (removed.ReviewGrades.Any())
            {
                removed.IsActive = false;
            }
            else
            {
                category.RatingCriteria.Remove(removed);
            }
        }
        
        category.Name = request.Name;
        category.Slug = request.Slug;
        category.SortOrder = request.SortOrder;
        category.Icon = request.Icon;
        category.ShowOnHomePage = request.ShowOnHomePage;

        await _categoryRepository.Update(category);

        return Unit.Value;
    }
}