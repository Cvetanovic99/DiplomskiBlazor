using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Cities.Queries;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyPages;

public partial class CompanySearch
{
    [Inject] public ICompanyDataService CompanyDataService { get; set; } = default!;
    [Inject] public ICityDataService CityDataService { get; set; } = default!;

    private const string MoreText = "...i još rezultata";

    private string? _addressText;
    private string? _companyText;

    private CityDto? _selectedAddress;
    private CompanyDto? _selectedCompany;

    private IEnumerable<CityDto> _addresses = new List<CityDto>();
    private IEnumerable<CompanyDto> _companies = new List<CompanyDto>();

    private async Task LoadAddresses(LoadDataArgs args)
    {
        var response = await InvokeDataServiceMethod(
            () => CityDataService.GetCities(
                args.Filter?.ToLower() ?? string.Empty,
                new QueryArgs { Take = 10, Skip = 0 }),
            errorMessage: "Greška pri učitavanju gradova");

        var items = response.Result?.Items?.ToList() ?? new List<CityDto>();

        if (response?.Result.TotalCount > 10)
        {
            items = items.Take(10).ToList();
            items.Add(new CityDto { Name = MoreText });
        }

        _addresses = items;
    }

    private async Task LoadCompanies(LoadDataArgs args)
    {
        if (_selectedAddress == null)
            return;

        var response = await InvokeDataServiceMethod(
            () => CompanyDataService.GetCompanies(
                args.Filter?.ToLower() ?? string.Empty,
                _selectedAddress.Id,
                new QueryArgs { Take = 10, Skip = 0 }),
            errorMessage: "Greška pri učitavanju kompanija");

        var items = response.Result?.Items?.ToList() ?? new List<CompanyDto>();

        if (response?.Result.TotalCount > 10)
        {
            items = items.Take(10).ToList();
            items.Add(new CompanyDto { Name = MoreText });
        }

        _companies = items;
    }

    private void OnAddressChanged(object value)
    {
        var text = value?.ToString();

        if (text == MoreText)
        {
            _selectedAddress = null;
            _addressText = null;
            return;
        }

        _addressText = text;
        _selectedAddress = _addresses.FirstOrDefault(x => x.Name == text);

        _selectedCompany = null;
        _companyText = null;
    }

    private void OnCompanyChanged(object value)
    {
        var text = value?.ToString();

        if (text == MoreText)
        {
            _selectedCompany = null;
            _companyText = null;
            return;
        }

        _companyText = text;
        _selectedCompany = _companies.FirstOrDefault(x => x.Name == text);
    }

    private void OnSubmit()
    {
        if (_selectedCompany == null)
            return;

        NavigationManager.NavigateTo($"/companies/{_selectedCompany.Id}");
    }

    private void GoToCreateCompany()
    {
        NavigationManager.NavigateTo("/createCompany");
    }
    
    private void GoHome()
    {
        NavigationManager.NavigateTo("/");
    }
}