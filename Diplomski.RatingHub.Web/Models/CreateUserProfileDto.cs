namespace Diplomski.RatingHub.Web.Models;

public class CreateUserProfileDto
{
    public required string IdentityUserId { get; set; }
    public required string Name { get; set; } 
    public required string Surname { get; set; }
    public string?   PhoneNumber    { get; set; }
    public string?   Email    { get; set; }
}