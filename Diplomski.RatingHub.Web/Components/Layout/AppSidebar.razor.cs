using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.Layout;

public partial class AppSidebar
{
    [Parameter] public bool IsExpanded { get; set; }
    [Inject] public ICurrentUserService CurrentUserService { get; set; }
    [Inject] public IUserProfileDataService UserProfileDataService { get; set; }

    private bool _unreadNotifications;

    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            var authenticatedUser = await CurrentUserService.GetAuthenticatedUserAsync();

            if (authenticatedUser != null)
            {
                var result = await InvokeDataServiceMethod(
                    () => UserProfileDataService.CheckForNewNotifications(authenticatedUser.UserProfileId),
                    errorMessage:"Doslo je do greske prilikom ocenjivanja, molimo vas pokusajte kasnije");

                if (!result.ExceptionOccurred)
                {
                    _unreadNotifications = result.Result;
                }
            }
        }
    }

    private void NotificationsClicked()
    {
        NavigationManager.NavigateTo("/user/notifications");
        
        _unreadNotifications = false;
    }
}