namespace Diplomski.RatingHub.Domain.Models;

public class CompanyRatingAggregate : EntityBase
{
    public int RatingsCount { get; set; }//Number of reviews
    public double AverageValue { get; set; }//SumValue / RatingsCount
    public int SumValue { get; set; }//Sum of all grades
    
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public int RatingCriterionId { get; set; }
    public RatingCriterion RatingCriterion { get; set; } = null!;
}