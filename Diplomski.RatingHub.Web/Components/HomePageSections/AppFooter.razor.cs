using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.HomePageSections;

public partial class AppFooter : ComponentBase
{
    protected int CurrentYear => DateTime.Now.Year;
}