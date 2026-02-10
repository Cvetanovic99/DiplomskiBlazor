namespace Diplomski.RatingHub.Domain.Models;

public class CompanyRatingAggregate : EntityBase
{
    public int RatingsCount { get; set; }
    public decimal AverageValue { get; set; }
    public int SumValue { get; set; }
    
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public int RatingCriterionId { get; set; }
    public RatingCriterion RatingCriterion { get; set; } = null!;
}