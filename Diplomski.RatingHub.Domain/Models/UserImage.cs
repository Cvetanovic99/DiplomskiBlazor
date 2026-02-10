namespace Diplomski.RatingHub.Domain.Models;

public class UserImage : ImageBase
{
    public int UserId { get; set; }
    public UserProfile User { get; set; } = null!;
}