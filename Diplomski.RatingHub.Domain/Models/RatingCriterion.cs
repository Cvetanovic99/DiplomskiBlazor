namespace Diplomski.RatingHub.Domain.Models;

public class RatingCriterion : EntityBase
{
    public required string Name { get; set; } 
    public int SortOrder { get; set; } 
    public bool IsActive { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
}