using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.ReportedContents.Queries;

public class GetReportedContentsQuery : IRequest<IPaginatedList<ReportedContentDto>>
{
    public string Search { get; set; }
    public ReportedContentEntityType? Type { get; set; }
    public ReportedContentStatus? Status { get; set; }
    public QueryArgs QueryArgs { get; set; }
}

public class GetReportedContentsQueryValidator : AbstractValidator<GetReportedContentsQuery>
{
    public GetReportedContentsQueryValidator()
    {
        RuleFor(x => x.QueryArgs).NotNull();
    }
}

public class GetReportedContentsQueryHandler : IRequestHandler<GetReportedContentsQuery, IPaginatedList<ReportedContentDto>>
{
    private readonly IDatabaseRepository<ReportedContent> _repo;

    public GetReportedContentsQueryHandler(IDatabaseRepository<ReportedContent> repo)
    {
        _repo = repo;
    }

    public async Task<IPaginatedList<ReportedContentDto>> Handle(GetReportedContentsQuery request, CancellationToken cancellationToken)
    {
        var spec = new Specification<Domain.Models.ReportedContent>(
            r => r.Title.Contains(request.Search) ||
                 r.Reason.Contains(request.Search));

        if (request.Type.HasValue)
            spec.And(r => r.ReportedEntityType == request.Type.Value.ToString());
        
        if (request.Status.HasValue)
            spec.And(r => r.Status == request.Status);

        return await _repo.GetAndProjectAsPaginatedList<ReportedContentDto>(spec, request.QueryArgs);
    }
}

public class ReportedContentDto : IMapFrom<ReportedContent>
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ContentUrl { get; set; }
    public string Reason { get; set; }
    public int ReportedEntityId { get; set; }
    public int? ReportedUserId { get; set; }
    public ReportedContentStatus Status { get; set; }
    public string ReportedEntityType { get; set; }
    public DateTime Created { get; set; }
}