using Diplomski.RatingHub.Application.UseCases.ReportedContents.Queries;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.AdminPages.ReportedContentPages;

public partial class ReportedContentDetails
{
    [Parameter] public ReportedContentDto Model { get; set; }

    [Inject] public IReportedContentDataService DataService { get; set; } = null!;
    [Inject] public ICompanyDataService CompanyDataService { get; set; } = null!;
    [Inject] public IReviewDataService ReviewDataService { get; set; } = null!;
    [Inject] public IUserProfileDataService UserProfileDataService { get; set; } = null!;

    private bool _confirmDelete = false;
    private bool _confirmBlockUser = false;

    override protected async Task OnInitializedAsync()
    {
        Model.ReportedUserId = 354;
    }

    private async Task Delete()
    {
        if (Enum.TryParse<ReportedContentEntityType>(Model.ReportedEntityType, out var value))
        {
            if (value == ReportedContentEntityType.Company)
            {
                var res = await InvokeDataServiceMethod(() =>
                        CompanyDataService.DeleteCompanyAsAnonymous(Model.ReportedEntityId, true),
                    successMessage: "Uspesno ste izbriasali kompaniju");

                if (!res)
                {
                    ShowNotification("Doslo je do greske prilikom brisanja kompanije", NotificationSeverity.Error);
                }
                else
                    _confirmDelete = false;
            }
            else if (value == ReportedContentEntityType.Review)
            {
                var res = await InvokeDataServiceMethod(() =>
                        ReviewDataService.DeleteReview(Model.ReportedEntityId),
                    successMessage: "Uspesno ste izbrisali ocenjivanje");

                if (!res)
                {
                    ShowNotification("Doslo je do greske prilikom brisanja ocenjivanja", NotificationSeverity.Error);
                } 
                else
                    _confirmDelete = false;
            } 
            else if (value == ReportedContentEntityType.CompanyResponse)
            {
                //TODO Delete CompanyResponse when that's implemented
            }
        }
        else
        {
            ShowNotification("Doslo je do greske, molimo vas pokusajte kasnije", NotificationSeverity.Error);
        }
        
    }

    private async Task BlockUser()
    {
        var res = await InvokeDataServiceMethod(() =>
                UserProfileDataService.BlockUserProfile(Model.ReportedUserId!.Value),
            successMessage: "Uspesno ste blokirali korisnika");

        if (!res)
        {
            ShowNotification("Doslo je do greske prilikom blokiranja korisnika", NotificationSeverity.Error);
        }
        else
            _confirmBlockUser = false;
    }
}