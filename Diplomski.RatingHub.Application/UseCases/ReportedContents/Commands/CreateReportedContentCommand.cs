using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.ReportedContents.Commands;

public class CreateReportedContentCommand : IRequest<Unit>
{
    public string Title { get; set; }
    public string Reason { get; set; }
    public string ReportedEntityType { get; set; }
    public int ReportedEntityId { get; set; }
    public string? ContactEmail  { get; set; }
    public string ContentUrl { get; set; }
    public int? ReportedUserId { get; set; }
    public int? ReporterUserId { get; set; }
}

public class CreateReportedContentCommandValidator : AbstractValidator<CreateReportedContentCommand>
{
    public CreateReportedContentCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Naslov je obavezan");
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Razlog je obavezan");
        RuleFor(x => x.ReportedEntityType).NotEmpty().WithMessage("Tip entiteta je obavezan");
        RuleFor(x => x.ReportedEntityId).GreaterThan(0).WithMessage("Id entiteta je obavezan");
    }
}

public class CreateReportedContentCommandHandler : IRequestHandler<CreateReportedContentCommand, Unit>
{
    private readonly IDatabaseRepository<ReportedContent> _repository;

    public CreateReportedContentCommandHandler(IDatabaseRepository<ReportedContent> repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(CreateReportedContentCommand request, CancellationToken cancellationToken)
    {
        var reportedContent = new ReportedContent
        {
            Title = request.Title,
            Reason = request.Reason,
            ReportedEntityType = request.ReportedEntityType,
            ReportedEntityId = request.ReportedEntityId,
            ContactEmail = request.ContactEmail,
            Status = ReportedContentStatus.Pending,
            ContentUrl = request.ContentUrl,
            ReportedUserId = request.ReportedUserId,
            ReporterUserId = request.ReporterUserId
        };

        await _repository.Insert(reportedContent);
        
        return Unit.Value;
    }
}
