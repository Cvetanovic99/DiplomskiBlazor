namespace Diplomski.RatingHub.Web.Models;

public class CreateCompanyDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string Verifier { get; set; }
    public string? ClaimCompanyIdentifier { get; set; }
    public string? AnonymousEditIdentifier { get; set; }
    public bool IsEmailVerifier { get; set; }
    public string? PublicPageUrl  { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? CompanyPib { get; set; }
    public int? OwnerId { get; set; }
    public int CategoryId { get; set; }
    public int CityId { get; set; }
    
    public ICollection<CreateImageDto> Images { get; set; } = new List<CreateImageDto>();
}

public class CreateCompanyAsAnonymousResponse
{
    public int CompanyId { get; set; }
    public required string AnonymousEditIdentifier { get; set; }
}