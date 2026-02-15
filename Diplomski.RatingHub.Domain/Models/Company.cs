namespace Diplomski.RatingHub.Domain.Models;

public class Company : EntityBase
{
    public required string Name { get; set; }
    public int ReviewsCount { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; } //Maybe village name
    public required string Street { get; set; }
    public required string Number { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsAnonymousCreator { get; set; }

    public int CreatorId { get; set; }
    public UserProfile Creator { get; set; } = null!;
    public int OwnerId { get; set; }
    public UserProfile Owner { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int CityId { get; set; }
    public City City { get; set; } = null!;
    
    public ICollection<CompanyImage> Images { get; set; } = new List<CompanyImage>();
    public ICollection<CompanyRatingAggregate> CompanyRatingAggregates { get; set; } = new List<CompanyRatingAggregate>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<CompanyResponse> Responses { get; set; } = new List<CompanyResponse>();
}