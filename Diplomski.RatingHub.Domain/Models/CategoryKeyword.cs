namespace Diplomski.RatingHub.Domain.Models;

public class CategoryKeyword : EntityBase
{
    public required string Keyword { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}