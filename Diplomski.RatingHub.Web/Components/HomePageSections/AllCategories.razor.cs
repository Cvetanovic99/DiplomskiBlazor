using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.HomePageSections;

public partial class AllCategories
{
    [Parameter] public IEnumerable<SubcategoryDto> Categories { get; set; } = [];
    
    [Inject] public ICategoryDataService CategoryDataService { get; set; } =  null!;


    private async Task OnExpand(TreeExpandEventArgs args)
    {
        // Ovde kasnije možeš da dohvatiš children iz baze kada se čvor proširi.
        // Za sada koristimo postojeće Children podatke.
        var category = args.Value as SubcategoryDto;
        
        args.Children.Data = await GetSubcategories(category.Id);
        args.Children.TextProperty = "Name";
        args.Children.HasChildren = (category) => (category as SubcategoryDto).HasChildren;
    }

    private async Task<IEnumerable<SubcategoryDto>> GetSubcategories(int parentCategoryId)
    {
        var response = await InvokeDataServiceMethod(
            () => CategoryDataService.GetSubcategories(parentCategoryId),
            errorMessage: "Greška prilikom ucitavanja podkategorija");

        if (!response.ExceptionOccurred)
        {
            return response.Result.Items;
        }
        
        return [];
    }

    private async Task OnChange(TreeEventArgs args)
    {
        if (args.Value is SubcategoryDto subcategory)
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
                NavigationManager.NavigateTo($"/companies?CityId={cityId}&CategoryId={subcategory.Id}");
            }
        }
        else
        {
            ShowNotification("Doslo je do greske", NotificationSeverity.Error);
        }
    }
}