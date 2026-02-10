namespace Diplomski.RatingHub.Domain.Models;

public class City : EntityBase
{
    public required string Name { get; set; } 
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public ICollection<Company>  Companies { get; set; } = new List<Company>();
}