namespace Diplomski.RatingHub.Domain.Models;

public class ReviewGrade : EntityBase
{
    public int Grade { get; set; }
    
    public int ReviewId { get; set; }
    public Review Review { get; set; } = null!;
    public int RatingCriterionId { get; set; }
    public RatingCriterion RatingCriterion { get; set; } = null!;
}