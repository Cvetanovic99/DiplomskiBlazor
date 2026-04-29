using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.ReportedContents.Queries;
using Diplomski.RatingHub.Domain.Enums;

namespace Diplomski.RatingHub.Web.Data.Interfaces;

public interface IReportedContentDataService
{
    Task<IPaginatedList<ReportedContentDto>> GetReportedContents(string search, ReportedContentEntityType? type, 
        ReportedContentStatus? status, QueryArgs args);
    Task DeleteReportedContent(int id);
    Task EditReportedContent(int id, ReportedContentStatus status);
}