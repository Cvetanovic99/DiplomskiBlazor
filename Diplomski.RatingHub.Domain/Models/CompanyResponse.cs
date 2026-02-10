namespace Diplomski.RatingHub.Domain.Models;

public class CompanyResponse : EntityBase
{
    public required string Text { get; set; }
    
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    public int ReviewId { get; set; }
    public Review Review { get; set; } = null!;
    
    public ICollection<CompanyResponseImage> Images { get; set; } = new List<CompanyResponseImage>();
}