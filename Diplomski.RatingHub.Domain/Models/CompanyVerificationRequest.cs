using Diplomski.RatingHub.Domain.Enums;

namespace Diplomski.RatingHub.Domain.Models;

public class CompanyVerificationRequest : EntityBase
{
    public CompanyVerificationRequestStatus Status { get; set; }
    public required string ContactEmail { get; set; }
    public string? Description { get; set; }
    public required string Identifier { get; set; }//Unique identifier for this verification request
    
    public int OwnerId { get; set; }
    public UserProfile Owner { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; }
}