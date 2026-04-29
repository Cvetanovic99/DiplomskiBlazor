using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.ReportedContents.Commands;
using Diplomski.RatingHub.Application.UseCases.ReportedContents.Queries;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Web.Data.Interfaces;

namespace Diplomski.RatingHub.Web.Data.Services;

public class ReportedContentDataService(IServiceScopeFactory serviceScopeFactory) : DataServiceBase(serviceScopeFactory),  IReportedContentDataService
{
    public async Task<IPaginatedList<ReportedContentDto>> GetReportedContents(string search, ReportedContentEntityType? type, 
        ReportedContentStatus? status, QueryArgs args)
    {
        return await Send(new GetReportedContentsQuery
        {
            Search = search,
            Type = type,
            Status = status,
            QueryArgs = args
        });
    }

    public async Task DeleteReportedContent(int id)
    {
        await Send(new DeleteReportedContentCommand { Id = id });
    }

    public async Task EditReportedContent(int id, ReportedContentStatus status)
    {
         await Send(new EditReportedContentCommand { Id = id, Status = status });
    }
}