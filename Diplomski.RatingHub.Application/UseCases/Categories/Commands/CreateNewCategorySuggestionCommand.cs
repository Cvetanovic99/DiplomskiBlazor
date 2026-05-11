using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Categories.Commands;

public class CreateNewCategorySuggestionCommand : IRequest<Unit>
{
    public required string CategoryName { get; set; }
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
}

public class CreateNewCategorySuggestionCommandValidator : AbstractValidator<CreateNewCategorySuggestionCommand>
{
    public CreateNewCategorySuggestionCommandValidator()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty().WithMessage("Naziv kategorije je obavezan")
            .MaximumLength(200).WithMessage("Naziv kategorije ne može biti duži od 200 karaktera");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Opis ne može biti duži od 1000 karaktera")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.ParentCategoryId)
            .GreaterThan(0).WithMessage("ID roditeljske kategorije mora biti veći od 0")
            .When(x => x.ParentCategoryId.HasValue);
    }
}

public class CreateNewCategorySuggestionCommandHandler : IRequestHandler<CreateNewCategorySuggestionCommand, Unit>
{
    private readonly IDatabaseRepository<NewCategorySuggestion> _newCategorySuggestionRepository;
    private readonly IDatabaseRepository<Category> _categoryRepository;

    public CreateNewCategorySuggestionCommandHandler(
        IDatabaseRepository<NewCategorySuggestion> newCategorySuggestionRepository,
        IDatabaseRepository<Category> categoryRepository)
    {
        _newCategorySuggestionRepository = newCategorySuggestionRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Unit> Handle(CreateNewCategorySuggestionCommand request, CancellationToken cancellationToken)
    {
        if (request.ParentCategoryId.HasValue)
        {
            var parentCategory = await _categoryRepository.GetById(request.ParentCategoryId.Value);
            if (parentCategory == null)
                throw new AppException("Roditeljska kategorija ne postoji");
        }
        

        var suggestion = new NewCategorySuggestion
        {
            Name = request.CategoryName,
            Text = request.Description,
            Status = NewCategorySuggestionStatus.Pending,
            ParentCategoryId = request.ParentCategoryId
        };

        await _newCategorySuggestionRepository.Insert(suggestion);

        return Unit.Value;
    }
}
