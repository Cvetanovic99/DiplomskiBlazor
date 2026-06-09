using Diplomski.RatingHub.Application.Interfaces.Notifications;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Commands;

public class ProcessSponsoredCompaniesCommand : IRequest<Unit>
{
}

public class ProcessSponsoredCompaniesCommandHandler : IRequestHandler<ProcessSponsoredCompaniesCommand, Unit>
{
    private readonly IDatabaseRepository<Company> _companyRepository;
    private readonly IDatabaseRepository<Notification> _notificationRepository;
    private readonly IEmailNotificationService _emailNotificationService;
    private readonly ISmsNotificationService _smsNotificationService;

    public ProcessSponsoredCompaniesCommandHandler(IDatabaseRepository<Company> companyRepository,
        IDatabaseRepository<Notification> notificationRepository,
        IEmailNotificationService emailNotificationService,
        ISmsNotificationService smsNotificationService)
    {
        _companyRepository = companyRepository;
        _notificationRepository = notificationRepository;
        _emailNotificationService = emailNotificationService;
        _smsNotificationService = smsNotificationService;
        
    }

    public async Task<Unit> Handle(ProcessSponsoredCompaniesCommand request, CancellationToken cancellationToken)
    {
        TimeZoneInfo serbiaZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
       var now =  DateTime.UtcNow;
       var tomorrow  =  now.Date.AddDays(1);

       //Expired companies
       var expiredCompanies = await _companyRepository.Get(
           new Specification<Company>(c => c.IsSponsored && c.SponsoredUntil != null && c.SponsoredUntil <= now));

       foreach (var company in expiredCompanies)
       {
           company.IsSponsored = false;
           company.SponsoredUntil = null;
       }
       
       //Soon Expiring companies
       var soonCompanies = await _companyRepository.Get(
           new Specification<Company>(c => c.IsSponsored && 
                                           c.SponsoredUntil != null && 
                                           c.SponsoredUntil >= tomorrow && 
                                           c.SponsoredUntil < tomorrow.AddDays(1))
               .AddInclude(c => c.Owner));
       
       var systemNotifications = new List<Notification>();
       foreach (var company in soonCompanies)
       {
           DateTime serbiaTime = TimeZoneInfo.ConvertTimeFromUtc(company.SponsoredUntil!.Value, serbiaZone);
           string expirationDate = serbiaTime.ToString("MMMM dd, yyyy HH:mm", new System.Globalization.CultureInfo("sr-Latn-RS"));
            
           if (company.Owner is not null)
           {
               systemNotifications.Add(new Notification
               {
                   Title = "Istek sponzorstva kompanije",
                   Message = $"Vasoj kompaniji '{company.Name}' istice sponzorisanje: {expirationDate}",
                   RecipientId = company.Owner.Id,
                   EntityType = nameof(Company),
                   IsRead = false
               });
           }
           
           if (IsEmail(company.Verifier))
           {
               await _emailNotificationService.NotifyOwnerAboutSponsoredCompanyExpirationAsync(company.Verifier, company.Name, expirationDate);
           }
           else
           {
               await _smsNotificationService.NotifyOwnerAboutSponsoredCompanyExpiration(company.Name, expirationDate);//Just for testing purposes, it goes over Email
           }
       }

       await _companyRepository.UpdateRange(expiredCompanies);
       await _notificationRepository.InsertRange(systemNotifications);
       
       return Unit.Value;
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
}