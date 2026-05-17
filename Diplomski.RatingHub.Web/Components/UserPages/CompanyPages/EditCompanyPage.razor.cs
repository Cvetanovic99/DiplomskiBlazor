using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Application.UseCases.Cities.Queries;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Web.Components.Shared;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using Diplomski.RatingHub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyPages;

public partial class EditCompanyPage
{
   [Parameter] public int CompanyId { get; set; }

    [Inject] public IHttpService HttpService { get; set; } = null!;
    [Inject] public ICityDataService CityDataService { get; set; } = null!;
    [Inject] public ICategoryDataService CategoryDataService { get; set; } = null!;
    [Inject] public ICompanyDataService CompanyDataService { get; set; } = null!;

    private EditCompanyDto Model = new();

    private IList<CityDto> _cities = new List<CityDto>();
    private string _selectedCityText;

    private IEnumerable<CategoryWithBreadCrumbDto> _categories = new List<CategoryWithBreadCrumbDto>();
    private string _selectedCategoryText;

    
    private List<EditCompanyImageDto> _existingImages = new();
    private List<string> _imagesToDelete = new();
    private List<CreateImageDto> _newImages = new();

    public const string _companyImageUrl = "company-image";
    private bool IsLoadingSubmit;

    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {

            var response = await InvokeDataServiceMethod(
                () => CompanyDataService.GetCompanyForEdit(CompanyId),
                errorMessage: "Greška prilikom učitavanja kompanije");

            if (!response.ExceptionOccurred)
            {
                Model = response.Result;

                _existingImages = Model.Images.ToList();

                _selectedCityText = Model.City.Name;
                _selectedCategoryText = Model.CategoryName;
                
                _cities.Add(Model.City);
            }
        }
    }

    // =========================
    // CITY
    // =========================
    private async Task LoadCities(LoadDataArgs args)
    {
        var response = await InvokeDataServiceMethod(
            () => CityDataService.GetCities(
                args.Filter?.ToLower() ?? string.Empty,
                new QueryArgs { Take = 10, Skip = 0 }),
            errorMessage: "Greška prilikom ucitavanja gradova");

        if (!response.ExceptionOccurred)
            _cities = response.Result.Items.ToList();
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

        int cityId = selectedCity?.Id ?? 0;
        if (Model.CityId != cityId)
        {
            Model.Longitude = 0.0;
            Model.Latitude = 0.0;
        }
        Model.CityId = cityId;
        StateHasChanged();
    }

    private void OnCitySelected() => StateHasChanged();

    // =========================
    // CATEGORY
    // =========================
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

    private void OnCategorySelected() => StateHasChanged();

    // =========================
    // MAP
    // =========================
    private async Task OpenMap()
    {
        if (Model.CityId == 0)
        {
            ShowNotification("Prvo izaberite grad", NotificationSeverity.Error);
            return;
        }

        var city = _cities.FirstOrDefault(x => x.Id == Model.CityId);

        var result = await DialogService.OpenAsync<CreateCompanyMapDialog>(
            "Izaberi lokaciju",
            new Dictionary<string, object>
            {
                { "CityLocation", new CreateCompanyMapDialog.MapDataDto { Latitude = city.Latitude, Longitude = city.Longitude } },
                { "CompanyLocation", new CreateCompanyMapDialog.MapDataDto { Latitude = Model.Latitude, Longitude = Model.Longitude } }
            },
            new DialogOptions { Width = "70%", Style = "margin-top: 130px" });

        if (result is CreateCompanyMapDialog.MapDataDto mapResult)
        {
            Model.Latitude = mapResult.Latitude;
            Model.Longitude = mapResult.Longitude;
        }
    }

    // =========================
    // UPLOAD
    // =========================
    private async Task HandleFiles(InputFileChangeEventArgs e)
    {
        if (_existingImages.Count + _newImages.Count + e.FileCount > 10)
        {
            ShowNotification("Max 10 slika", NotificationSeverity.Error);
            return;
        }

        var files = e.GetMultipleFiles(10);

        foreach (var file in files)
        {
            var content = ProcessImage(file);
            if (content.ExceptionOccured)
            {
                ShowNotification($"Slika: {file.Name} je prevelika, maksimalna velicina je '500 KB.'", NotificationSeverity.Error);
                continue;
            }

            var response = await HttpService.UploadImage(content.Content!, _companyImageUrl);
            if (response.ExceptionOccurred)
            {
                ShowNotification("Greška pri uploadu slike", NotificationSeverity.Error);
                continue;
            }

            _newImages.Add(new CreateImageDto
            {
                Path = response.Result!.Path,
                Title = response.Result!.Title,
                IsProfile = false
            });
        }

        EnsureProfileImage();
    }

    private ProcessImageResponse ProcessImage(IBrowserFile file)
    {
        var response = new ProcessImageResponse();
        try
        {
            var content = new MultipartFormDataContent();
            var stream = file.OpenReadStream(5_000_000);
            content.Add(new StreamContent(stream), "file", file.Name);
            
            response.Content = content;
            response.ExceptionOccured = false;
        }
        catch (Exception e)
        {
            response.Content = null;
            response.ExceptionOccured = true;
        }
        
        return response;
    }
    
    // =========================
    // REMOVE
    // =========================
    private void RemoveExistingImage(EditCompanyImageDto img)
    {
        _existingImages.Remove(img);
        _imagesToDelete.Add(img.Path);

        EnsureProfileImage();
    }

    private async Task RemoveNewImage(CreateImageDto img)
    {
        var success = await HttpService.DeleteImage(_companyImageUrl, img.Path);

        if (success)
            _newImages.Remove(img);

        EnsureProfileImage();
    }

    // =========================
    // SUBMIT
    // =========================
    private async Task HandleSubmit(EditCompanyDto model)
    {
        IsLoadingSubmit = true;
        EnsureProfileImage();

        model.Images = _existingImages
            .Select(x => new EditCompanyImageDto
            {
                Path = x.Path,
                Title = x.Title,
                IsProfile = x.IsProfile,
                SortOrder = x.SortOrder
            })
            .Concat(_newImages.Select(x => new EditCompanyImageDto
            {
                Path = x.Path,
                Title = x.Title,
                IsProfile = x.IsProfile
            }))
            .ToList();

        var result = await InvokeDataServiceMethod(
            () => CompanyDataService.EditCompany(model),
            errorMessage: "Greška pri izmeni");

        IsLoadingSubmit = false;
        if (result)
        {
            foreach (var path in _imagesToDelete)
                await HttpService.DeleteImage(_companyImageUrl, path);

            if (Model.OwnerId is not null)//If owner edited company
                NavigationManager.NavigateTo($"/user/companies/");
            else
                NavigationManager.NavigateTo($"/companies/{CompanyId}");
        }
    }

    // =========================
    // CANCEL
    // =========================
    private async Task GoBack()
    {
        foreach (var img in _newImages)
        {
            await HttpService.DeleteImage(_companyImageUrl, img.Path);
        }

        NavigationManager.NavigateTo($"/companies/{CompanyId}");
    }

    // =========================
    // VALIDATION
    // =========================
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
    
    private void EnsureProfileImage()
    {
        // reset svega
        foreach (var img in _existingImages)
            img.IsProfile = false;

        foreach (var img in _newImages)
            img.IsProfile = false;

        // ako ima existing prva je profilna
        if (_existingImages.Any())
        {
            _existingImages[0].IsProfile = true;
            return;
        }

        // ako nema existing  uzmi iz novih
        if (_newImages.Any())
        {
            _newImages[0].IsProfile = true;
        }
    }
}