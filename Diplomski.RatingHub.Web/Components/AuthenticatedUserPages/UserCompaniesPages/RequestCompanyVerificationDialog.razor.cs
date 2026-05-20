using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserCompaniesPages;

public partial class RequestCompanyVerificationDialog
{
    [Parameter] public int UserProfileId { get; set; }
    [Parameter] public int CompanyId { get; set; }
    
    [Inject] public ICompanyVerificationRequestDataService CompanyVerificationRequestDataService { get; set; } =  null!;
    
    
    private VerifyCompanyRequestDto model = new();


    private async Task OnSubmit()
    {
        var response = await InvokeDataServiceMethod(
            () => CompanyVerificationRequestDataService
                .CreateVerificationRequestStatus(UserProfileId, CompanyId, model.ContactEmail, model.Description));

        if (!response.ExceptionOccurred)
        {
            DialogService.Close(response.Result);
        }
    }

    private void OnCancel()
    {
        DialogService.Close(null);
    }
    
    private class VerifyCompanyRequestDto
    {
        public string ContactEmail { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}