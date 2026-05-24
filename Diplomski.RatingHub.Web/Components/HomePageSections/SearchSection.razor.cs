using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Application.UseCases.Cities.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.HomePageSections;

public partial class SearchSection
{
    [Parameter] public string SectionId { get; set; } = "home-search-section";

    [Inject] public ICityDataService CityDataService { get; set; } = null!;
    [Inject] public ICategoryDataService CategoryDataService { get; set; } = null!;
    
    
    private IEnumerable<CityDto> _cities = new List<CityDto>();
    private string? _selectedCityText;
    private int _selectedCityId;
    
    private IEnumerable<CategoryOrCompanyDto> _categoriesAndCompanies = new List<CategoryOrCompanyDto>();
    private string? _selectedCategoryText;
    private CategoryOrCompanyDto? _selectedCategoryOrCompany = null;
    

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
            _selectedCityId = 0;
            _selectedCategoryOrCompany = null;
            _selectedCategoryText = null;
            StateHasChanged();
            return;
        }

        _selectedCityText = text;

        var selectedCity = _cities
            .FirstOrDefault(x => string.Equals(x.Name, text, StringComparison.CurrentCultureIgnoreCase));

        _selectedCityId = selectedCity?.Id ?? 0;
        StateHasChanged();
    }
    
    private void OnCitySelected()
    {
        StateHasChanged();
    }
    
    private async Task LoadCategoriesAndCompanies(LoadDataArgs args)
    {
        var res = await InvokeDataServiceMethod(
            () => CategoryDataService.GetCategoriesAndCompanies(_selectedCityId, args.Filter?.ToLower() ?? string.Empty, new QueryArgs { Take = 10, Skip = 0 }),
            errorMessage: "Greška prilikom ucitavanja");

        if (!res.ExceptionOccurred)
            _categoriesAndCompanies = res.Result?.ToList();
    }
    
    private void OnCategoryChanged(object value)
    {
        var text = value?.ToString();

        if (string.IsNullOrWhiteSpace(text))
        {
            _selectedCategoryText = null;
            _selectedCategoryOrCompany = null;
            StateHasChanged();
            return;
        }

        _selectedCategoryText = text;

         _selectedCategoryOrCompany = _categoriesAndCompanies
            .FirstOrDefault(x =>string.Equals(x.Name, text, StringComparison.CurrentCultureIgnoreCase));
        
        StateHasChanged();
    }
    
    private void OnCategorySelected()
    {
        StateHasChanged();
    }

    private void ExecuteSearch()
    {
        if (_selectedCategoryOrCompany is null || _selectedCityId == 0)
        {
            ShowNotification("Morate izabrati grad i kategoriju/kompaniju");
            return;
        }

        if (_selectedCategoryOrCompany!.IsCategory)
            NavigationManager.NavigateTo($"/companies?CityId={_selectedCityId}&CategoryId={_selectedCategoryOrCompany.Id}");
        else
            NavigationManager.NavigateTo($"/companies/{_selectedCategoryOrCompany.Id}");
    }

    protected class SearchSuggestionItem
    {
        public string Label { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
    }
}