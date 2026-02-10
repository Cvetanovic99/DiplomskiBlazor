using Diplomski.RatingHub.Domain.Enums;

namespace Diplomski.RatingHub.Domain.Models;

public class ReportedContent : EntityBase
{
    public required string Reason { get; set; }
    public required string ReportedEntityType { get; set; }
    public int ReportedEntityId { get; set; }
    public ReportedContentStatus Status { get; set; }
    
    public int ReportedUserId { get; set; }
    public UserProfile ReportedUser { get; set; } = null!;
    public int ReporterUserId { get; set; }
    public UserProfile ReporterUser { get; set; } =  null!;
}