using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.Layout;

public partial class AppSidebar : ComponentBase
{
    [Parameter] public bool IsExpanded { get; set; }
}