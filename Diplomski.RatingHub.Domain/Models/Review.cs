namespace Diplomski.RatingHub.Domain.Models;

public class Review : EntityBase
{
    public required string Comment { get; set; }
    public decimal OverallScore { get; set; }
    public bool IsAnonymousReview { get; set; }
    
    public int  ReviewerId { get; set; }
    public UserProfile Reviewer { get; set; } =  null!;
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public CompanyResponse? CompanyResponse { get; set; }
    
    public ICollection<ReviewImage> Images { get; set; } = new List<ReviewImage>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<ReviewGrade> Grades { get; set; } = new List<ReviewGrade>();
}