using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Notifications;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Companies.Commands;
using Diplomski.RatingHub.Application.UseCases.CompanyVerifications.Commands;
using Diplomski.RatingHub.Application.UseCases.CompanyVerifications.Queries;
using Diplomski.RatingHub.Application.UseCases.Notifications.Commands;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Domain.Models;
using Diplomski.RatingHub.Web.Data.Interfaces;

namespace Diplomski.RatingHub.Web.Data.Services;

public class CompanyVerificationRequestDataService : DataServiceBase, ICompanyVerificationRequestDataService
{
    private readonly IEmailNotificationService _emailNotificationService;
    
    public CompanyVerificationRequestDataService(
        IServiceScopeFactory serviceScopeFactory, 
        IEmailNotificationService emailNotificationService) : base(serviceScopeFactory)
    {
        _emailNotificationService = emailNotificationService;
    }
    public async Task<IPaginatedList<CompanyVerificationRequestDto>> GetVerificationRequests(string search, CompanyVerificationRequestStatus? status, QueryArgs args)
    {
        return await Send(new GetCompanyVerificationRequestsQuery
            {
                Search = search,
                Status = status,
                QueryArgs = args
            });
    }

    public async Task DeleteVerificationRequest(int requestId)
    {
        await Send(new DeleteCompanyVerificationRequestCommand { RequestId = requestId });
    }

    public async Task SendCompanyVerificationRulesToUser(string userEmail, int userId, string identifier, string companyName)
    {
        await _emailNotificationService.SendCompanyVerificationRulesAsync(userEmail, identifier, companyName);
        
        await Send(new CreateNotificationCommand
        {
            Title = "Verifikacija kompanije",
            Message = $"Poslat vam je email sa pravilima verifikacije kompanije {companyName}",
            RecipientId = userId,
            EntityType = nameof(CompanyVerificationRequest)
        });
    }

    public async Task VerifyCompany(int companyId, int recipiendId)
    {
        await Send(new VerifyCompanyCommand { CompanyId = companyId });
        await Send(new CreateNotificationCommand
        {
            Title = "Verifikacija kompanije",
            Message = $"Vaša kompanija je uspešno verifikovana",
            RecipientId = recipiendId,
            EntityType = nameof(CompanyVerificationRequest)
        });
    }

    public async Task EditVerificationRequestStatus(int requestId, CompanyVerificationRequestStatus newStatus)
    {
        await Send(new EditCompanyVerificationRequestStatusCommand
        {
            RequestId = requestId,
            Status = newStatus
        });
    }
}