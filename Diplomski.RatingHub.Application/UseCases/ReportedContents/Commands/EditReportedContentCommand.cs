using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.ReportedContents.Commands;

public class EditReportedContentCommand : IRequest<Unit>
{
    public int Id { get; set; }
    public ReportedContentStatus Status { get; set; }
}

public class EditReportedContentCommandValidator : AbstractValidator<EditReportedContentCommand>
{
    public EditReportedContentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class EditReportedContentCommandHandler : IRequestHandler<EditReportedContentCommand, Unit>
{
    private readonly IDatabaseRepository<ReportedContent> _repo;

    public EditReportedContentCommandHandler(IDatabaseRepository<ReportedContent> repo)
    {
        _repo = repo;
    }

    public async Task<Unit> Handle(EditReportedContentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repo.GetById(request.Id);

        if (entity is null)
            throw new ApplicationException("Ne postoji prijava");

        entity.Status = request.Status;

        await _repo.Update(entity);

        return Unit.Value;
    }
}