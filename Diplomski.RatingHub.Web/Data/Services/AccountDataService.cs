using System.Text;
using System.Text.Encodings.Web;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Notifications;
using Diplomski.RatingHub.Application.Models.Notifications;
using Diplomski.RatingHub.Domain.Constants;
using Diplomski.RatingHub.Infrastructure.Auth.Enums;
using Diplomski.RatingHub.Infrastructure.Auth.Models;
using Diplomski.RatingHub.Web.Components.Account.Pages;
using Diplomski.RatingHub.Web.Constants;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Diplomski.RatingHub.Web.Data.Services;

public class AccountDataService : DataServiceBase, IAccountDataService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailNotificationService _emailNotificationService;
    private readonly IUserProfileDataService _userProfileDataService;
    private readonly ISmsNotificationService _smsNotificationService;
    
    public AccountDataService(IMediator mediator,  
        UserManager<ApplicationUser> userManager,
        IEmailNotificationService emailNotificationService,
        IUserProfileDataService userProfileDataService,
        ISmsNotificationService smsNotificationService,IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    {
        _userManager = userManager;
        _emailNotificationService = emailNotificationService;
        _userProfileDataService = userProfileDataService;
        _smsNotificationService = smsNotificationService;
    }

    public async Task<RegisterUserResult> RegisterUser(RegisterUserDto registerUserDto)
    {
        var identityUser = await CreateIdentityUser(registerUserDto);

        try
        {
            if (registerUserDto.RegistrationMethod == RegistrationMethod.Email)
            {
                string emailConfirmationLink = await CreateEmailConfirmationLink(identityUser.Email!);
                await _emailNotificationService.SendConfirmationLinkAsync(identityUser.Email!, emailConfirmationLink);
            }
            else if(registerUserDto.RegistrationMethod == RegistrationMethod.Phone)
            {
                var result = await CreatePhoneNumberConfirmationToken(identityUser.UserName!);
                //await _smsNotificationService.SendConfirmationToken(identityUser.PhoneNumber!, phoneNumberConfirmationToken);
                await _smsNotificationService.SendConfirmationTokenWithEmail(result.Token);
            }
        }
        catch (Exception e)
        {
            await DeleteIdentityUserAsync(identityUser.Id);
            throw new AppException("Postovani trenutno ne mozemo da kreiramo vas nalog, molimo da pokusate kasnije");
        }

        var userProfile = await _userProfileDataService.CreateUserProfile(new CreateUserProfileDto
        {
            IdentityUserId = identityUser.Id,
            Name = registerUserDto.Name,
            Surname = registerUserDto.Surname,
            PhoneNumber = registerUserDto.RegistrationMethod is RegistrationMethod.Phone
                ? registerUserDto.Verifier
                : null,
            Email = registerUserDto.RegistrationMethod is RegistrationMethod.Email ? registerUserDto.Verifier : null,
        });

        return new RegisterUserResult
        {
            RegistrationMethod = identityUser.RegistrationMethod, 
            UserIdentityId = identityUser.Id,
            Verifier = identityUser.UserName!
        };
    }

    private async Task<ApplicationUser> CreateIdentityUser(RegisterUserDto registerUserDto)
    {
        var user = new ApplicationUser
        {
            UserName = registerUserDto.Verifier,
            Email = registerUserDto.RegistrationMethod is RegistrationMethod.Email ? registerUserDto.Verifier : null,
            PhoneNumber = registerUserDto.RegistrationMethod is RegistrationMethod.Phone ? registerUserDto.Verifier : null,
            RegistrationMethod = registerUserDto.RegistrationMethod,
        };
        
        var result = await _userManager.CreateAsync(user, registerUserDto.Password);
        
        if (!result.Succeeded)
        {
            var message = result.Errors
                .Select(e => e.Description)
                .Aggregate((current, next) => $"{current} {next}");

            throw new Exception(message);
        }
    
        var roleResult = await _userManager.AddToRoleAsync(user, Roles.RegularUser);

        if (!roleResult.Succeeded)
        {
            var message = roleResult.Errors
                .Select(e => e.Description)
                .Aggregate((current, next) => $"{current} {next}");

            throw new Exception(message);
        }

        return user;
    }

    public async Task ResendEmailConfirmationLink(string email)
    {
        string emailConfirmationLink = await CreateEmailConfirmationLink(email);
        try
        {
            await _emailNotificationService.SendConfirmationLinkAsync(email, emailConfirmationLink);
        }
        catch
        {
            throw new AppException(
                "Trenutno nismo u mogucnosti da posaljemo link za potvrdu email adrese, molimo vas pokusajte kasnije");
        }
    }
    
    public async Task<string> ResendPhoneNumberConfirmationToken(string phoneNumber)
    {
        var result = await CreatePhoneNumberConfirmationToken(phoneNumber);
        try
        {
            //await _smsNotificationService.SendConfirmationToken(identityUser.PhoneNumber!, phoneNumberConfirmationToken);
            await _smsNotificationService.SendConfirmationTokenWithEmail(result.Token);
            return result.IdentityId;
        }
        catch
        {
            throw new AppException(
                "Trenutno nismo u mogucnosti da posaljemo poruku za potvrdu broja telefona, molimo vas pokusajte kasnije");
        }
    }

    private async Task<string> CreateEmailConfirmationLink(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) 
            throw new AppException("Korisnik sa trazenom email adresom nije pronadjen");

        string emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        
        emailConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));
        if (emailConfirmationToken is null)
            throw new AppException("Trenutno nismo u mogucnosti da posaljemo link za potvrdu email adrese, molimo vas pokusajte kasnije");
        
        string callbackUrl = $"{BaseApplicationUrls.BaseHttpUrl}/Account/ConfirmEmail?userId={user.Id}&code={emailConfirmationToken}";
        
        return HtmlEncoder.Default.Encode(callbackUrl);
    }

    private async Task<(string Token, string IdentityId)> CreatePhoneNumberConfirmationToken(string phoneNumber)
    {
        var user = await _userManager.FindByNameAsync(phoneNumber);
        if (user is null) 
            throw new AppException("Korisnik sa trazenim brojem telefona nije pronadjen");
        
        var phoneNumberConfirmationToken = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber!);
        if (phoneNumberConfirmationToken is null)
            throw new AppException("Trenutno nismo u mogucnosti da posaljemo poruku za potvrdu broja telefona, molimo vas pokusajte kasnije");
        
        return (phoneNumberConfirmationToken, user.Id);
    }

    private async Task DeleteIdentityUserAsync(string identityUserId)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null) 
            throw new AppException("User not found");
        
        await _userManager.DeleteAsync(user);
    }
}