using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.ReportedContents.Commands;

public class DeleteReportedContentCommand : IRequest<Unit>
{
    public int Id { get; set; }
}

public class DeleteReportedContentCommandValidator : AbstractValidator<DeleteReportedContentCommand>
{
    public DeleteReportedContentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

public class DeleteReportedContentCommandHandler : IRequestHandler<DeleteReportedContentCommand, Unit>
{
    private readonly IDatabaseRepository<ReportedContent> _repo;

    public DeleteReportedContentCommandHandler(
        IDatabaseRepository<ReportedContent> repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(DeleteReportedContentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repo.GetById(request.Id);

        if (entity is null)
            throw new ApplicationException("Prijava ne postoji");

        await _repo.Delete(entity);

        return Unit.Value;
    }
}