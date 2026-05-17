namespace Diplomski.RatingHub.Domain.Models;

public class Company : EntityBase
{
    public required string Name { get; set; }
    public int ReviewsCount { get; set; }
    public double OverallAverageGrade { get; set; }
    public double SumGradesValue { get; set; } //Sum of average values from all reviews
    public string? Description { get; set; }
    public string? Location { get; set; } //Maybe village name
    public required string Street { get; set; }
    public required string HouseNumber { get; set; }
    public required string Verifier { get; set; }//Phonenumber or Email
    public bool IsEmailVerifier { get; set; }//Is email, if not then it's phonenumber
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsAnonymousCreator { get; set; }
    public string? CompanyPib { get; set; }//If company is registered
    public string? PublicPageUrl  { get; set; }//If company has some public page website, instagram, facebook
    public bool IsVerified { get; set; }//Is verified with video-admin
    public string? AnonymousEditIdentifier { get; set; }//If someone create company anonymously and whant to edit
    public string? ClaimCompanyIdentifier { get; set; }//When someone whant to claim company using this identifier-code
    //public int CreatorId { get; set; }
    //public UserProfile Creator { get; set; } = null!;
    public int? OwnerId { get; set; }
    public UserProfile? Owner { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public int CityId { get; set; }
    public City City { get; set; } = null!;
    
    public ICollection<CompanyImage> Images { get; set; } = new List<CompanyImage>();
    public ICollection<CompanyRatingAggregate> CompanyRatingAggregates { get; set; } = new List<CompanyRatingAggregate>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<CompanyResponse> Responses { get; set; } = new List<CompanyResponse>();
    public ICollection<CompanyVerificationRequest> VerificationRequests { get; set; } = new List<CompanyVerificationRequest>();
}