using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.Layout;

public partial class AppHeader : ComponentBase
{
    [Parameter] public EventCallback OnToggleDesktopSidebar { get; set; }

    private async Task ToggleDesktopSidebar()
    {
        await OnToggleDesktopSidebar.InvokeAsync();
    }

    private static string GetDisplayName(System.Security.Claims.ClaimsPrincipal user)
    {
        return user.Identity?.Name?.Trim() switch
        {
            { Length: > 0 } name => name,
            _ => "Moj nalog"
        };
    }
}