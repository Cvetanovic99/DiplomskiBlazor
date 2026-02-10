namespace Diplomski.RatingHub.Domain.Models;

public class CompanyImage : ImageBase
{
    public int SortOrder { get; set; }
    public bool IsProfile { get; set; }
    
    public int CompanyId { get; set; }
    public Company Company { get; set; } = null!;
}