using Diplomski.RatingHub.Domain.Enums;
using MediatR;
using FluentValidation;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;

namespace Diplomski.RatingHub.Application.UseCases.Categories.Commands;

public class EditNewCategorySuggestionCommand: IRequest<Unit>
{
    public int NewCategorySuggestionId { get; set; }
    public NewCategorySuggestionStatus Status { get; set; }
}

public class EditNewCategorySuggestionCommandValidator : AbstractValidator<EditNewCategorySuggestionCommand>
{
    public EditNewCategorySuggestionCommandValidator()
    {
        RuleFor(x => x.NewCategorySuggestionId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class EditNewCategorySuggestionCommandHandler : IRequestHandler<EditNewCategorySuggestionCommand, Unit>
{
    private readonly IDatabaseRepository<NewCategorySuggestion> _newCategorySuggestionRepository;

    public EditNewCategorySuggestionCommandHandler(IDatabaseRepository<NewCategorySuggestion> newCategorySuggestionRepository)
    {
        _newCategorySuggestionRepository = newCategorySuggestionRepository;
    }

    public async Task<Unit> Handle(EditNewCategorySuggestionCommand request, CancellationToken cancellationToken)
    {
        var suggestion = await _newCategorySuggestionRepository.GetSingleBySpec(new Specification<NewCategorySuggestion>(
            s => s.Id == request.NewCategorySuggestionId));
        
        if (suggestion is null)
            throw new AppException("Predlog kategorije ne postoji");

        suggestion.Status = request.Status;

        await _newCategorySuggestionRepository.Update(suggestion);

        return Unit.Value;
    }
}
