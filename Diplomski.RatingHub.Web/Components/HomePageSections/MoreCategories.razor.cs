using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.HomePageSections;

public partial class MoreCategories
{
    [Parameter] public IEnumerable<TopCategoryDto> Categories { get; set; } = [];
    
    private async Task OnCategoryClick(TopCategoryDto category)
    {
        var result = await DialogService.OpenAsync<SelectCityDialog>(
            "Izbor grada",
            options: new DialogOptions
            {
                Width = "500px",
                Height = "auto",
                Style = "margin-top: 130px"
            });

        if (result != null)
        {
            int cityId = (int)result;
            NavigationManager.NavigateTo($"/companies?CityId={cityId}&CategoryId={category.Id}");
        }
    }
}