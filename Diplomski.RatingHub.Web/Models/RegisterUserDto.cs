using System.Text.RegularExpressions;
using Diplomski.RatingHub.Infrastructure.Auth.Enums;

namespace Diplomski.RatingHub.Web.Models;

public class RegisterUserDto
{
    public required string Name { get; set; } 
    public required string Surname { get; set; }
    public required string Verifier { get; set; }
    public required string Password { get; set; }
    public RegistrationMethod RegistrationMethod => ComputeRegistrationMethod(Verifier);

    private RegistrationMethod ComputeRegistrationMethod(string verifier)
    {
        if (IsEmail(verifier))
        {
            return RegistrationMethod.Email;
        }
        else if (IsPhone(verifier))
        {
            return RegistrationMethod.Phone;
        }
        else
        {
            throw new Exception("Nevalidan broj telefona ili email!");
        }
    }

    private bool IsEmail(string value)
    {
        try
        {
            _ = new System.Net.Mail.MailAddress(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private bool IsPhone(string value)
    {
        return Regex.IsMatch(value, @"^06[0-9]{7,8}$");
    }
}