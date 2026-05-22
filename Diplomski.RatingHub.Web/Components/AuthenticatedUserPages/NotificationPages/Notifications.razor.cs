using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Notifications.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using Diplomski.RatingHub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.NotificationPages;

public partial class Notifications
{
    [Inject] public INotificationDataService NotificationDataService { get; set; }
    [Inject] public ICurrentUserService  CurrentUserService { get; set; } = null!;

    private AuthenticatedUserDto _authenticatedUser;
    private List<NotificationDto> _notifications = new();
    string pagingSummaryFormat = "Str. {0} od {1} (ukupno {2} obavestenja)";

    private int _pageSize = 15;
    private int _totalCount;
    private int _currentPage = 0;

    protected override async Task OnInitializedAsync()
    {
        await GetCurrentUser();
        await LoadNotifications();
    }

    private async Task LoadNotifications(int skip = 0)
    {
        if (skip == 0)
            _currentPage = 0;

        var res = await InvokeDataServiceMethod(
            () => NotificationDataService.GetUserNotifications(_authenticatedUser.UserProfileId,
                new QueryArgs { Skip = skip, Take = _pageSize, OrderBy = "Created desc"}),
            errorMessage: "Greška pri učitavanju");

        if (!res.ExceptionOccurred)
        {
            _notifications = res.Result.Items.ToList();
            _totalCount = res.Result.TotalCount;
        }
    }
    
    private async Task GetCurrentUser()
    {
        var currentUser = await CurrentUserService.GetAuthenticatedUserAsync();
        if (currentUser == null)
        {
            ShowNotification("Doslo je do greske prilikom ucitavanja korisnika", NotificationSeverity.Error);
            return;
        }
        _authenticatedUser = currentUser;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnPageChanged(PagerEventArgs args)
    {
        await LoadNotifications(args.Skip);
    }

    private async Task DeleteAllNotifications()
    {
        var res = await DialogService.Confirm("Da li ste sigurni da želite da obrišete sva obaveštenja?","Brisanje obavestenja",
            new ConfirmOptions { OkButtonText = "Izbrisi", CancelButtonText = "Odustani", ShowClose = false });
        if (res is true)
        {
            var result = await InvokeDataServiceMethod(
                () => NotificationDataService.DeleteAllUserNotifications(_authenticatedUser.UserProfileId),
                "Greška pri brisanju");
            
            if (result)
            {
                ShowNotification("Uspesno ste izbrisali sva obavestenja", NotificationSeverity.Success);
                await LoadNotifications();
            }
        }
    }
    
    private string FormatDate(DateTime date)
    {
        return date.ToString("MMM d, yyyy HH'h'", new System.Globalization.CultureInfo("sr-Latn-RS"));
    }
}