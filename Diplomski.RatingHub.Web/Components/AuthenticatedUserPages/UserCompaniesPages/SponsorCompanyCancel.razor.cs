using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserCompaniesPages;

public partial class SponsorCompanyCancel
{
    private void GoBack()
    {
        NavigationManager.NavigateTo("user/companies");
    }
}