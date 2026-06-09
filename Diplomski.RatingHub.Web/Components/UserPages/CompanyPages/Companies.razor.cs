using Diplomski.RatingHub.Application.Enums;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Application.UseCases.Cities.Queries;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Domain.Models;
using Diplomski.RatingHub.Web.Components.Shared;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyPages;

public partial class Companies 
{
    [SupplyParameterFromQuery] public int? CityId { get; set; }
    [SupplyParameterFromQuery] public int? CategoryId { get; set; }
    
    [Inject] public ICompanyDataService CompanyDataService { get; set; } = null!;
    [Inject] public ICategoryDataService CategoryDataService { get; set; } = null!;
    [Inject] public ICityDataService CityDataService { get; set; } = null!;
    
    private IEnumerable<FilteredCompanyDto> _companies = new List<FilteredCompanyDto>();
    private int _companiesTotalCount;
    private bool IsCompanyLoading;

    private List<CategoryParentDto> _breadcrumbs = new List<CategoryParentDto>();
    
    private string _companyFiltervalue;
    private double _minOverallAverageGradeFilterValue;
    
    private CompanyClaimStatusFilterOptions _claimStatusFilterValue = CompanyClaimStatusFilterOptions.Sve;
    
    private CompanyVerificationStatusFilterOptions _verificationStatusFilterValue = CompanyVerificationStatusFilterOptions.Sve;
    
    private CompanySortingOptions _sortingOption = CompanySortingOptions.Najnovije;
    private IEnumerable<CompanySortingOptions> _sortingOptions = Enum.GetValues<CompanySortingOptions>();
    private int _currentPage;
    private int _pageSize = 10;
    
    private IEnumerable<SubcategoryDto> _subcategories = new  List<SubcategoryDto>();
    private int _subcategoriesTotalCount;

    private CityDto _city;
    
    string pagingSummaryFormat = "Str. {0} od {1} (ukupno {2} kompanija)";

    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            if (CategoryId is null || CityId is null || CityId == 0 || CategoryId == 0)
            {
                ShowNotification("Doslo je do greške", NotificationSeverity.Error);
            }
            else
            {
                await LoadSubcategories();
                await LoadCompanies();
                await LoadBreadcrumbs();
                await LoadCityAndMap();
            }
        }
    }

    private async Task LoadSubcategories()
    {
        var queryArgs = new QueryArgs { Take = 10, Skip = 0 , OrderBy = $"{nameof(SubcategoryDto.SortOrder)} asc" };
        var response = await InvokeDataServiceMethod(
            () => CategoryDataService.GetSubcategories(CategoryId.Value, queryArgs),
            errorMessage: "Greška pri učitavanju podkategorija");

        if (!response.ExceptionOccurred)
        {
            _subcategories = response.Result.Items;
            _subcategoriesTotalCount = response.Result.TotalCount;
        }
    }

    private async Task LoadCompanies(int skip = 0)
    {
        if (skip == 0)
            _currentPage = 0;
        
        IsCompanyLoading = true;

        var res = await InvokeDataServiceMethod(
            () => CompanyDataService.GetFilteredCompanies(
                CityId.Value,
                CategoryId.Value,
                _companyFiltervalue,
                _minOverallAverageGradeFilterValue,
                new QueryArgs { Skip = skip, Take = _pageSize},
                _claimStatusFilterValue,
                _verificationStatusFilterValue,
                GetOrderBy()), errorMessage: "Greška pri učitavanju");

        if (!res.ExceptionOccurred)
        {
            _companies = res.Result.Items;
            _companiesTotalCount = res.Result.TotalCount;
        }

        IsCompanyLoading = false;

        await RenderMarkers();
    }

    private async Task LoadBreadcrumbs()
    {
        var response = await InvokeDataServiceMethod(
            () => CategoryDataService.GetCategoryParents(CategoryId.Value),
            errorMessage: "Greška pri učitavanju roditeljskih kategorija");

        if (!response.ExceptionOccurred)
            _breadcrumbs = response.Result.ToList();
    }
    
    private async Task LoadCityAndMap()
    {
        var response = await InvokeDataServiceMethod(
            () => CityDataService.GetCityById(CityId.Value),
            errorMessage: "Greška pri učitavanju grada");

        if (!response.ExceptionOccurred)
            _city = response.Result;
        
        await InitMap();
        await RenderMarkers();
    }
    
    private async Task InitMap()
    {
        await JSRuntime.InvokeVoidAsync("initMap", _city.Latitude, _city.Longitude, 13);
    }

    private async Task RenderMarkers()
    {
        var mapData = GetCompaniesForMap();

        await JSRuntime.InvokeVoidAsync("setCompaniesOnMap", mapData);
    }

    private IEnumerable<MapCompanyDto> GetCompaniesForMap()
    {
        return _companies.Select(c => new MapCompanyDto
        {
            Id = c.Id,
            Latitude = c.Latitude.Value,
            Longitude = c.Longitude.Value,
            Name = c.Name,
            Rating = c.OverallAverageGrade,
            Reviews = c.ReviewsCount,
            Address = $"{c.City}, {c.Street} {c.HouseNumber}",
            ImageUrl = c.ProfileImagePath ?? "/images/companyImages/genericCompanyImage.svg"
            
        });
    }

    private void OnSubcategorySelected(int categoryId)
    {
        CategoryId = categoryId;
        
        NavigationManager.NavigateTo($"/companies?CityId={CityId}&CategoryId={categoryId}", true);
    }

    private async Task OpenAllSubcategoriesDialog()
    {
        var result = await DialogService.OpenAsync<SubcategoriesDialog>(
            "Sve podkategorije",
            new Dictionary<string, object>
            {
                { "ParentCategoryId", CategoryId.Value }
            },
            new DialogOptions
            {
                Width = "70%",
                Height = "50vh",
                Style = "margin-top: 130px"
            });

        if (result is int subcategoryId)
        {
            OnSubcategorySelected(subcategoryId);
        }
    }
    
    private async Task OpenSuggestNewSubcategoryDialog()
    {
        
        var result = await DialogService.OpenAsync<SuggestNewCategory>(
            "Predložite novu podkategoriju",
            new Dictionary<string, object>
            {
                { "ParentCategoryId", CategoryId }
            },
            new DialogOptions
            {
                Width = "500px",
                Height = "auto",
                Style = "margin-top: 130px"
            });

        if (result is true)
        {
            ShowNotification("Uspesno ste poslali predlog", NotificationSeverity.Success);
        }
    }

    private async Task OnPageChanged(PagerEventArgs args)
    {
        await LoadCompanies(args.Skip);
    }

    private async Task OnFilterChanged(ChangeEventArgs args)
    {
        _companyFiltervalue = $"{args.Value}".Trim();
        await LoadCompanies(0);
    }
    
    private async Task OnOverallAverageGradeFilterChanged(double value)
    {
        await LoadCompanies(0);
    }

    private async Task OnSortingChange()
    {
        await LoadCompanies(0);
    }

    private string GetOrderBy()
    {
        string value = "";
        switch (_sortingOption)
        {
            case CompanySortingOptions.Najbolje:
                value = $"{nameof(FilteredCompanyDto.OverallAverageGrade)} desc";
                break;
            case CompanySortingOptions.Najlosije:
                value = $"{nameof(FilteredCompanyDto.OverallAverageGrade)} asc";
                break;
            case CompanySortingOptions.Najnovije:
                value = $"{nameof(FilteredCompanyDto.Created)} desc";
                break;
            case CompanySortingOptions.Najstarije:
                value = $"{nameof(FilteredCompanyDto.Created)} asc";
                break;
            default:
                break;
        }

        return value;
    }

    private string GetSortingOptions(object value)
    {
        var option = (CompanySortingOptions)value;
        switch (option)
        {
            case CompanySortingOptions.Najbolje:
                return "Ocene opadajuce";
            case CompanySortingOptions.Najlosije: 
                return "Ocene rastuce";
            case CompanySortingOptions.Najnovije:
                return "Novije";
            case CompanySortingOptions.Najstarije:
                return "Starije";
            default:
                return "";
        }
    }

    private void GoToCreateCompany()
    {
        NavigationManager.NavigateTo("/createCompany");
    }

    private void OnBreadcrumbClicked(int categoryId)
    {
        NavigationManager.NavigateTo($"/companies?CityId={CityId}&CategoryId={categoryId}", true);
    }

    private async Task OpenMapModal()
    {
        await DialogService.OpenAsync<CompanyMapCard>(
             "Mapa kompanija",
             new Dictionary<string, object>
             {
                 { "City", _city },
                 { "Companies", GetCompaniesForMap() }
             },
             new DialogOptions
             {
                 Height = "80vh",
                 Width = "80vw",
                 Style = "margin-top: 90px"
             });
        
    }

    private async Task OpenFilterModal()
    {
        var result = await DialogService.OpenAsync<CompaniesFilterDialog>(
            "Filteri",
            new Dictionary<string, object?>
            {
                {
                    "InitialModel",
                    new CompaniesFilterDto
                    {
                        OverallAverageGrade = _minOverallAverageGradeFilterValue,
                        ClaimStatus = _claimStatusFilterValue,
                        VerificationStatus = _verificationStatusFilterValue
                    }
                }
            },
            new DialogOptions
            {
                Width = "70%",
                Height = "50vh",
                Style = "margin-top: 130px"
            });
        
        if (result is CompaniesFilterDto filters)
        {
            _minOverallAverageGradeFilterValue = filters.OverallAverageGrade;
            _claimStatusFilterValue = filters.ClaimStatus;
            _verificationStatusFilterValue = filters.VerificationStatus;

            await LoadCompanies(0);
        }
    }

    private void GoToHome()
    {
        NavigationManager.NavigateTo("/");
    }
    
    public class MapCompanyDto
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Name { get; set; }
        public double Rating { get; set; }
        public int Reviews { get; set; }
        public string Address { get; set; }
        public string ImageUrl  { get; set; }
    }
}