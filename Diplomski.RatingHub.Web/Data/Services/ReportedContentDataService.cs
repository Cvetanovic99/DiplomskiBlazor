using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.ReportedContents.Commands;
using Diplomski.RatingHub.Application.UseCases.ReportedContents.Queries;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Web.Components.Shared;
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

    public async Task CreateReportedContent(ReportContentDialog.ReportContentDto reportContentDto)
    {
        await Send(new CreateReportedContentCommand
        {
            Title = reportContentDto.Title,
            Reason = reportContentDto.Reason,
            ReportedEntityType = reportContentDto.ReportedEntityType.ToString(),
            ReportedEntityId = reportContentDto.ReportedEntityId,
            ContactEmail = reportContentDto.Email,
            ContentUrl = CreateReportedContentUrl(reportContentDto.ReportedEntityType,  reportContentDto.ReportedEntityId, reportContentDto.ReviewId),
            ReportedUserId = reportContentDto.ContentOwnerId,
            ReporterUserId = reportContentDto.ReporterUserId,
        });
    }

    private string CreateReportedContentUrl(ReportedContentEntityType reportedEntityType, int reportedEntityId, int? reviewId)
    {
        switch (reportedEntityType)
        {
            case ReportedContentEntityType.Company:
                return $"/companies/{reportedEntityId}";
            case  ReportedContentEntityType.Review:
                return $"/admin/reportedContent/reviews/{reportedEntityId}";
            case  ReportedContentEntityType.CompanyResponse:
                return $"/admin/reportedContent/reviews/{reviewId}";
            default:
                return string.Empty;
        }
    }
}