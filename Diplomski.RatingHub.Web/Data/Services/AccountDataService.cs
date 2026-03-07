using System.Text;
using System.Text.Encodings.Web;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Infrastructure.Auth.Enums;
using Diplomski.RatingHub.Infrastructure.Auth.Models;
using Diplomski.RatingHub.Web.Components.Account.Pages;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Diplomski.RatingHub.Web.Data.Services;

public class AccountDataService : DataServiceBase, IAccountDataService
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailSender<ApplicationUser> _emailSender;
    private readonly IUserProfileDataService _userProfileDataService;
    
    public AccountDataService(IMediator mediator,  
        UserManager<ApplicationUser> userManager,
        IEmailSender<ApplicationUser> emailSender,
        IUserProfileDataService userProfileDataService) : base(mediator)
    {
        _mediator = mediator;
        _userManager = userManager;
        _emailSender = emailSender;
        _userProfileDataService = userProfileDataService;
    }

    public async Task<RegisterUserResult> RegisterUser(RegisterUserDto registerUserDto)
    {
        var identityUser = await CreateIdentityUser(registerUserDto);

        try
        {
            if (registerUserDto.RegistrationMethod == RegistrationMethod.Email)
            {
                string emailConfirmationLink = await CreateEmailConfirmationLink(identityUser.Id);
                await _emailSender.SendConfirmationLinkAsync(identityUser, identityUser.Email!, emailConfirmationLink);
            }
            else if(registerUserDto.RegistrationMethod == RegistrationMethod.Phone)
            {
                
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

        return new RegisterUserResult { RegistrationMethod = identityUser.RegistrationMethod };
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
        
        return user;
    }

    private async Task<string> CreateEmailConfirmationLink(string identityUserId)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null) 
            throw new AppException("User not found");

        string emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        
        emailConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));
        if (emailConfirmationToken is null)
            throw new AppException("Unable to generate email confirmation token");
        
        string callbackUrl = $"Account/ConfirmEmail?userId={user.Id}&code={emailConfirmationToken}";
        
        return HtmlEncoder.Default.Encode(callbackUrl);
    }

    private async Task DeleteIdentityUserAsync(string identityUserId)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null) 
            throw new AppException("User not found");
        
        await _userManager.DeleteAsync(user);
    }
}