namespace Diplomski.RatingHub.Domain.Models;

public class Like : EntityBase
{
    public int ReviewId { get; set; }
    public Review Review { get; set; } = null!;
    public int UserId { get; set; }
    public UserProfile User { get; set; } = null!;
}