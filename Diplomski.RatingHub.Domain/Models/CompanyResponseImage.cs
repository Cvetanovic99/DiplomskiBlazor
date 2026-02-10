namespace Diplomski.RatingHub.Domain.Models;

public class CompanyResponseImage : ImageBase
{
    public int SortOrder { get; set; }
    
    public int CompanyResponseId { get; set; }
    public CompanyResponse CompanyResponse { get; set; } = null!;
}