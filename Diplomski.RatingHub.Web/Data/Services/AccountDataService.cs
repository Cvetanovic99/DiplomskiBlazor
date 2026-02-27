using System.Text;
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
    
    public AccountDataService(IMediator mediator,  UserManager<ApplicationUser> userManager) : base(mediator)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    public async Task<RegisterUserResult> RegisterUser(RegisterUserDto registerUserDto)
    {
        string identityUserId = await CreateIdentityUser(registerUserDto);

        try
        {
            if (registerUserDto.RegistrationMethod == RegistrationMethod.Email)
            {
                string emailConfirmationToken = await CreateEmailConfirmationTokenAsync(identityUserId);
                //SendEmail
            }
            else if(registerUserDto.RegistrationMethod == RegistrationMethod.Phone)
            {
                
            }
        }
        catch (Exception e)
        {
            //Delete IdentityUser
            Console.WriteLine(e);
            throw;
        }
        
        
    }

    private async Task<string> CreateIdentityUser(RegisterUserDto registerUserDto)
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
        
        return user.Id;
    }

    private async Task<string> CreateEmailConfirmationTokenAsync(string identityUserId)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user is null) 
            throw new Exception("User not found");

        string emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        
        emailConfirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));
        if (emailConfirmationToken is null)
            throw new Exception("Unable to generate email confirmation token");
        
        string callbackUrl = $"Account/ConfirmEmail?userId={user.Id}&code={emailConfirmationToken}";
        
        return callbackUrl;
    }
}