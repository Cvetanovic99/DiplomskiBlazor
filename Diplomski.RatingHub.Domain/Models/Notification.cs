namespace Diplomski.RatingHub.Domain.Models;

public class Notification : EntityBase
{
    public required string Title { get; set; }
    public required string Message { get; set; }
    public bool IsRead { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    
    public int RecipientId { get; set; }
    public UserProfile Recipient { get; set; } = null!;
    public int ActorId { get; set; }
    public UserProfile Actor { get; set; } = null!;
}