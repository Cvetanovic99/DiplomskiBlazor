using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Cities.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.HomePageSections;

public partial class SelectCityDialog
{
    [Parameter] public int CategoryId { get; set; }
    [Inject] public ICityDataService CityDataService { get; set; } = null!;
    
    private ModelDto model = new();
    
    private IEnumerable<CityDto> _cities = new List<CityDto>();
    private string? _selectedCityText;
    private void OnSubmit()
    {
        DialogService.Close(model.CityId);
    }
    
    private async Task LoadCities(LoadDataArgs args)
    {
        var response = await InvokeDataServiceMethod(
            () => CityDataService.GetCities(
                args.Filter?.ToLower() ?? string.Empty,
                new QueryArgs { Take = 10, Skip = 0 }),
            errorMessage: "Greška prilikom ucitavanja gradova");

        if(!response.ExceptionOccurred)
            _cities = response.Result.Items;
    }
    
    private void OnCityChanged(object value)
    {
        var text = value?.ToString();

        if (string.IsNullOrWhiteSpace(text))
        {
            _selectedCityText = null;
            model.CityId = 0;
            StateHasChanged();
            return;
        }

        _selectedCityText = text;

        var selectedCity = _cities
            .FirstOrDefault(x => string.Equals(x.Name, text, StringComparison.CurrentCultureIgnoreCase));

        model.CityId = selectedCity?.Id ?? 0;
        StateHasChanged();
    }
    
    private void OnCitySelected()
    {
        StateHasChanged();
    }

    private void OnCancel()
    {
        DialogService.Close(null);
    }
    
    private class ModelDto
    {
        public int CityId { get; set; }
    }
}