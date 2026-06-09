using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserCompaniesPages;

public partial class SponsorCompanySuccess
{
    [Inject] ICompanyDataService CompanyDataService { get; set; } = null!;

    private int _companyId;
    private bool _success;

    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

            if (int.TryParse(query["companyId"], out var companyId))
            {
                _companyId = companyId;
                var response = await InvokeDataServiceMethod(
                    () => CompanyDataService.SetCompanyAsSponsored(_companyId),
                    errorMessage: "Doslo je do greske, molimo vas pokusajte kasnije.");
            
            
                _success = response;
            }
            else
            {
                _success = false;
            }
        }
    }

    private void GoToCompany()
    {
        NavigationManager.NavigateTo($"user/companies");
    }
}