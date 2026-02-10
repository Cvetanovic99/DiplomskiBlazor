namespace Diplomski.RatingHub.Domain.Models;

public class ReviewImage : ImageBase
{
    public int SortOrder { get; set; }
    
    public int ReviewId { get; set; }
    public Review Review { get; set; } = null!;
}