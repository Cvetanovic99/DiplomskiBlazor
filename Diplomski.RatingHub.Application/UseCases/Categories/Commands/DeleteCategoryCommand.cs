using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Categories.Commands;

public class DeleteCategoryCommand : IRequest<Unit>
{
    public int CategoryId { get; set; }
}

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public readonly IDatabaseRepository<Category> _categoryRepository;
    public readonly IDatabaseRepository<CategoryKeyword> _categoryKeywordRepository;
    public readonly IDatabaseRepository<RatingCriterion> _ratingCriterionRepository;
    public readonly IDatabaseRepository<NewCategorySuggestion> _newCategorySuggestionRepository;

    public DeleteCategoryCommandHandler(
        IDatabaseRepository<Category> categoryRepository, 
        IDatabaseRepository<CategoryKeyword> categoryKeywordRepository,
        IDatabaseRepository<RatingCriterion> ratingCriterionRepository,
        IDatabaseRepository<NewCategorySuggestion> newCategorySuggestionRepository)
    {
        _categoryRepository = categoryRepository;
        _categoryKeywordRepository = categoryKeywordRepository;
        _ratingCriterionRepository = ratingCriterionRepository;
        _newCategorySuggestionRepository = newCategorySuggestionRepository;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetSingleBySpec(new Specification<Category>(c => c.Id == request.CategoryId)
            .AddInclude(c => c.Companies)
            .AddInclude(c => c.Subcategories)
            .AddInclude(c => c.Keywords)
            .AddInclude(c => c.RatingCriteria));
        
        if (category is null)
            throw new ApplicationException("Kategorija ne postoji");
        
        if (category.Companies.Any())
            throw new ApplicationException("Kategorija sadrzi kompanije unutar nje, ne moze biti izbrisana");
        
        if (category.Subcategories.Any())
            throw new ApplicationException("Kategorija sadrzi podkategorije, molimo vas prvo obrisite sve podkategorije");
        
        var suggestions = await _newCategorySuggestionRepository
            .Get(new Specification<NewCategorySuggestion>(s => s.ParentCategoryId == request.CategoryId));
        
        if (suggestions.Any())
            await _newCategorySuggestionRepository.DeleteRange(suggestions);
        
        await _categoryKeywordRepository.DeleteRange(category.Keywords);
        await _ratingCriterionRepository.DeleteRange(category.RatingCriteria);
        await _categoryRepository.Delete(category);
        
        return Unit.Value;
    }
}