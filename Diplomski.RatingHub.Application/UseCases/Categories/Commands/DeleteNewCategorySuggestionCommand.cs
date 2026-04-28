using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Categories.Commands;

public class DeleteNewCategorySuggestionCommand : IRequest<Unit>
{
    public int NewCategorySuggestionId { get; set; }
}

public class DeleteNewCategorySuggestionCommandValidator : AbstractValidator<DeleteNewCategorySuggestionCommand>
{
    public DeleteNewCategorySuggestionCommandValidator()
    {
        RuleFor(x => x.NewCategorySuggestionId).GreaterThan(0);
    }
}

public class DeleteNewCategorySuggestionCommandHandler : IRequestHandler<DeleteNewCategorySuggestionCommand, Unit>
{
    private readonly IDatabaseRepository<NewCategorySuggestion> _newCategorySuggestionRepository;

    public DeleteNewCategorySuggestionCommandHandler(IDatabaseRepository<NewCategorySuggestion> newCategorySuggestionRepository)
    {
        _newCategorySuggestionRepository = newCategorySuggestionRepository;
    }

    public async Task<Unit> Handle(DeleteNewCategorySuggestionCommand request, CancellationToken cancellationToken)
    {
        var suggestion = await _newCategorySuggestionRepository.GetSingleBySpec(new Specification<NewCategorySuggestion>(
            s => s.Id == request.NewCategorySuggestionId));

        if (suggestion is null)
            throw new ApplicationException("Predlog kategorije ne postoji");

        await _newCategorySuggestionRepository.Delete(suggestion);

        return Unit.Value;
    }
}