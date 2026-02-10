namespace Diplomski.RatingHub.Domain.Models;

public class UserProfile : EntityBase
{
    public required string IdentityUserId { get; set; }
    public required string Name { get; set; } 
    public required string Surname { get; set; }
    public string?   PhoneNumber    { get; set; }
    public string?   Email    { get; set; }
    public bool Blocked { get; set; }
    public bool IsAnonymous  { get; set; }
    
    public UserImage? ProfileImage { get; set; }
    
    public ICollection<Company> CreatedCompanies { get; set; } = new List<Company>();
    public ICollection<Company> OwningCompanies { get; set; } = new List<Company>();
    public ICollection<Like> LikedContent { get; set; } = new List<Like>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    
    
    
}