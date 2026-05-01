using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace Diplomski.RatingHub.Web.Components.HomePageSections;

public partial class HeroSection : ComponentBase
{
    private RadzenCarousel? carousel;

    [Parameter] public string SearchSectionId { get; set; } = "home-search-section";
    [Parameter] public string CreateCompanyUrl { get; set; } = "/company-search";
}