using System.Net;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Application.UseCases.Cities.Queries;
using Diplomski.RatingHub.Web.Components.Shared;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using Diplomski.RatingHub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyPages;

public partial class CreateCompany 
{
    [SupplyParameterFromQuery] private string? OwnerId { get; set; }
    [Inject] public IHttpService HttpService { get; set; } =  null!;
    [Inject] public ICityDataService CityDataService { get; set; } = null!;
    [Inject] public ICategoryDataService CategoryDataService { get; set; } = null!;
    [Inject] public ICompanyDataService CompanyDataService { get; set; } = null!;
    
    private CreateCompanyDto Model = new();

    private IEnumerable<CityDto> _cities = new List<CityDto>();
    private string _selectedCityText;

    private IEnumerable<CategoryWithBreadCrumbDto> _categories = new List<CategoryWithBreadCrumbDto>();
    private string _selectedCategoryText;

    private List<CreateImageDto> _images = new();
    

    public const string _companyImageUrl = "company-image";

    
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
            Model.CityId = 0;
            return;
        }

        _selectedCityText = text;

        var selectedCity = _cities
            .FirstOrDefault(x => string.Equals(x.Name, text, StringComparison.CurrentCultureIgnoreCase));

        Model.CityId = selectedCity?.Id ?? 0;
        StateHasChanged();
    }
    
    private async Task LoadCategories(LoadDataArgs args)
    {
        var res = await InvokeDataServiceMethod(
            () => CategoryDataService.GetCategoriesWithBreadCrumb(args.Filter?.ToLower() ?? string.Empty, 10),
            errorMessage: "Greška prilikom ucitavanja kategorija");

        if (!res.ExceptionOccurred)
            _categories = res.Result?.ToList();
    }

    private void OnCategoryChanged(object value)
    {
        var text = value?.ToString();

        if (string.IsNullOrWhiteSpace(text))
        {
            _selectedCategoryText = null;
            Model.CategoryId = 0;
            return;
        }

        _selectedCategoryText = text;

        var selectedCategory = _categories
            .FirstOrDefault(x => string.Equals(x.Name, text, StringComparison.CurrentCultureIgnoreCase));

        Model.CategoryId = selectedCategory?.Id ?? 0;
        StateHasChanged();
    }
    
    private void OnCategorySelected()
    {
        StateHasChanged();
    }
    
    private void OnCitySelected()
    {
        StateHasChanged();
    }
    
    private async Task OpenMap()
    {
        if (Model.CityId == 0)
        {
            ShowNotification("Prvo izaberite najblizi grad", NotificationSeverity.Error);
            return;
        }
        
        var city = _cities.FirstOrDefault(x => x.Id == Model.CityId);
        
        var result = await DialogService.OpenAsync<CreateCompanyMapDialog>(
            "Izaberi lokaciju",
            new Dictionary<string, object>
            {
                { "CityLocation", new CreateCompanyMapDialog.MapDataDto { Latitude = city.Latitude, Longitude = city.Longitude} },
                { "CompanyLocation", new CreateCompanyMapDialog.MapDataDto { Latitude = Model.Latitude, Longitude = Model.Longitude } }
            },
            new DialogOptions
            {
                Width = "70%",
                Style = "margin-top: 130px"
            });
        
        if (result is CreateCompanyMapDialog.MapDataDto mapResult)
        {
            Model.Latitude = mapResult.Latitude;
            Model.Longitude = mapResult.Longitude;
        }
    }
    
    
    private async Task HandleFiles(InputFileChangeEventArgs e)
    {
        if(_images.Count + e.FileCount > 10)
        {
            ShowNotification("Mozete uploadovati maksimalno 10 slika", NotificationSeverity.Error);
            return;
        }

        var files = e.GetMultipleFiles(10);
        
        foreach (var file in files)
        {
            var content = new MultipartFormDataContent();
            var stream = file.OpenReadStream(5_000_000);
        
            content.Add(new StreamContent(stream), "file", file.Name);
            
            var response = await HttpService.UploadImage(content, _companyImageUrl);
            if(response.ExceptionOccurred)    
            {
                ShowNotification("Greška pri uploadu slike " + file.Name, NotificationSeverity.Error);
                continue;
            }
            
            _images.Add(new CreateImageDto
            {
                Path = response.Result!.Path,
                Title = response.Result!.Title,
            });
        }
        
        if (_images.Any())
        {
            foreach (var img in _images)
                img.IsProfile = false;
        
            _images.First().IsProfile = true;
        }
    }

    private async Task RemoveImage(CreateImageDto img)
    {
        var response = await HttpService.DeleteImage(_companyImageUrl, img.Path);

        if (response)
        {
            _images.Remove(img);

            if (!_images.Any(x => x.IsProfile) && _images.Any())
                _images.First().IsProfile = true;
        }
        else
        {
                ShowNotification("Greška pri brisanju slike: " + img.Path, NotificationSeverity.Error);
        }
    }
    
    private async Task HandleSubmit(CreateCompanyDto model)
    {
        Model.Images = _images;
        
        if (string.IsNullOrEmpty(OwnerId))
        {
            var result = await InvokeDataServiceMethod(
                () => CompanyDataService.CreateCompanyAsAnonymous(Model),
                errorMessage: "Doslo je do greske prilikom kreiranja kompanije");

            if (!result.ExceptionOccurred)
            {
                var res = await DialogService.Alert(GetEditAnonymousIdentifierText(result.Result!.AnonymousEditIdentifier!), "Kod za azuriranje kompanije",
                    new AlertOptions() { OkButtonText = "Prikazi kompaniju", ShowClose = false });
                
                if(res is true)
                    NavigationManager.NavigateTo($"/companies/{result.Result.CompanyId}");
                
            }
        }
        else
        {
            if (!int.TryParse(OwnerId, out var id))
            {
                ShowNotification("URL nije validan, molimo vas da ponovite proces", NotificationSeverity.Error);
                return;
            }
            
            Model.OwnerId = id;
            
            var result = await InvokeDataServiceMethod(
                () => CompanyDataService.CreateCompanyAsOwner(Model),
                errorMessage: "Doslo je do greske prilikom kreiranja kompanije");

            if (!result.ExceptionOccurred)
            {
                NavigationManager.NavigateTo($"/user/companies/");
            }
        }
    }

    private async Task GoBack()
    {
        if(_images.Any())
        {
            foreach (var img in _images)
            {
                await HttpService.DeleteImage(_companyImageUrl, img.Path);
            }
        }
        
        NavigationManager.NavigateTo("/");
    }

    private bool ValidateVerifier()
    {
        if (Model.Verifier is null)
            return true;
        
        return RegisterUserDto.IsEmail(Model.Verifier) || RegisterUserDto.IsPhone(Model.Verifier);
    }
    
    private bool ValidatePublicPageUrl()
    {
        if (Model.PublicPageUrl is null)
            return true;
        
        return Model.PublicPageUrl.StartsWith("http://") || Model.PublicPageUrl.StartsWith("https://");
    }
}