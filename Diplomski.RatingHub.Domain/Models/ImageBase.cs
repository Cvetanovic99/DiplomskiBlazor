namespace Diplomski.RatingHub.Domain.Models;

public abstract class ImageBase : EntityBase
{
    public required string Title { get; set; }
    public required string Path { get; set; }
}