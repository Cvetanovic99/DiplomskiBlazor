using Diplomski.RatingHub.Domain.Enums;

namespace Diplomski.RatingHub.Domain.Models;

public class NewCategorySuggestion : EntityBase
{
    public required string Text { get; set; }
    public NewCategorySuggestionStatus Status { get; set; }
    
    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
}