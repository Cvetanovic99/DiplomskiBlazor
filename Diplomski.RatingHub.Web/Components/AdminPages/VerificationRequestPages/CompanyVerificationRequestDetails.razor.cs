using Diplomski.RatingHub.Application.UseCases.CompanyVerifications.Queries;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.AdminPages.VerificationRequestPages;

public partial class CompanyVerificationRequestDetails 
{
    [Parameter] public CompanyVerificationRequestDto Model { get; set; }

    [Inject] protected ICompanyVerificationRequestDataService DataService { get; set; }

    private bool _editStatus = false;
    private CompanyVerificationRequestStatus _newStatus;

    private bool _confirmVerify = false;
    private bool _contactMode = false;

    private string _contactEmail;

    protected override void OnInitialized()
    {
        _newStatus = Model.Status;
        _contactEmail = Model.ContactEmail;
    }

    private async Task UpdateStatus()
    {
        await InvokeDataServiceMethod(() =>
                DataService.EditVerificationRequestStatus(Model.Id, _newStatus),
            "Status izmenjen");

        Model.Status = _newStatus;
        _editStatus = false;
    }

    private void CancelEditStatus()
    {
        _newStatus = Model.Status;
        _editStatus = false;
    }

    private async Task VerifyCompany()
    {
        var result = await InvokeDataServiceMethod(() =>
                DataService.VerifyCompany(Model.CompanyId, Model.OwnerId, Model.CompanyName), 
            "Kompanija je uspesno verifikovana");

        if(result)
            _confirmVerify = false;
    }

    private async Task SendEmail()
    {
        var result = await InvokeDataServiceMethod(() =>
                DataService.SendCompanyVerificationRulesToUser(_contactEmail, 
                    Model.OwnerId,Model.Identifier!, Model.CompanyName!), "Email je uspesno poslat");

        if (result)
            _contactMode = false;
    }

    private void Close()
    {
        DialogService.Close();
    }

    private string GetUserImage()
    {
        return string.IsNullOrWhiteSpace(Model.UserProfileImagePath)
            ? "/images/userProfileImages/universalProfileImageAdmin.png"
            : Model.UserProfileImagePath;
    }
}