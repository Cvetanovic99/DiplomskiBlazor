namespace Diplomski.RatingHub.Domain.Models;

public class Review : EntityBase
{
    public required string Comment { get; set; }
    public string? ReviewerFullName { get; set; }//If reviewer is anonymous then it can put name and surname if he whants
    public double OverallScore { get; set; }
    public bool IsAnonymousReview { get; set; }
    public string? AnonymousEditIdentifier { get; set; }
    public bool IsCompanyDataTrue { get; set; }
    public required string ReviewerIdentifier { get; set; } //Ef IdentityId if user was logged in or my custom Guid if user was anonymous
    public int?  ReviewerId { get; set; }
    public UserProfile? Reviewer { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public int? CompanyResponseId { get; set; }
    public CompanyResponse? CompanyResponse { get; set; }
    
    public ICollection<ReviewImage> Images { get; set; } = new List<ReviewImage>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<ReviewGrade> Grades { get; set; } = new List<ReviewGrade>();
}