using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Diplomski.RatingHub.Web.CustomValidationAttributes;

public class EmailOrPhoneAttribute : ValidationAttribute
{
    private const string PhonePattern = @"^06[0-9]{7,8}$";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return new ValidationResult("Potrebno je uneti Korisnicko Ime.");

        var verifier = value.ToString()!;

        // Check email
        try
        {
            _ = new MailAddress(verifier);
            return ValidationResult.Success;
        }
        catch
        {
        }

        // Check phone
        if (Regex.IsMatch(verifier, PhonePattern))
            return ValidationResult.Success;

        return new ValidationResult("Korisnicko Ime mora biti validan Email ili broj telefona (06XXXXXXXX).");
    }
}