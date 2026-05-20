using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserCompaniesPages;

public partial class ClaimCompanyDialog
{
    [Parameter] public int UserProfileId { get; set; }
    
    [Inject] public ICompanyDataService CompanyDataService { get; set; } =  null!;
    
    
    private ClaimCompanyDto model = new();


    private async Task OnSubmit()
    {
        var response = await InvokeDataServiceMethod(
            () => CompanyDataService.SetCompanyOwner(UserProfileId, model.Code));

        if (response)
        {
            DialogService.Close(true);
        }
    }

    private void OnCancel()
    {
        DialogService.Close(false);
    }
    
    private class ClaimCompanyDto
    {
        public string Code { get; set; } = string.Empty;
    }
}