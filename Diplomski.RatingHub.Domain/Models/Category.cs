namespace Diplomski.RatingHub.Domain.Models;

public class Category : EntityBase
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public int SortOrder { get; set; }
    
    public int? ParentId  {get; set; }
    public Category? Parent { get; set; }
    
    public ICollection<Category> Subcategories { get; set; } = new List<Category>();
    public ICollection<CategoryKeyword> Keywords { get; set; } = new List<CategoryKeyword>();
    public ICollection<RatingCriterion> RatingCriteria { get; set; } = new List<RatingCriterion>();
    public ICollection<Company> Companies { get; set; } = new List<Company>();
}