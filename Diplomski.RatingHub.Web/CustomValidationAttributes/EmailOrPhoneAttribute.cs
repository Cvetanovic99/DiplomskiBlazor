using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Diplomski.RatingHub.Web.CustomValidationAttributes;

public class EmailOrPhoneAttribute : ValidationAttribute
{
    private const string PhonePattern = @"^06[0-9]{7,8}$";

    public EmailOrPhoneAttribute()
    {
        ErrorMessage = "Korisničko ime mora biti validan email ili broj telefona (06XXXXXXXX).";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var verifier = value?.ToString()?.Trim();
        
        if (string.IsNullOrWhiteSpace(verifier))
            return ValidationResult.Success;

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

        
        return new ValidationResult(
            ErrorMessage,
            validationContext.MemberName is null
                ? null
                : new[] { validationContext.MemberName });
    }
}