using System.Security.Claims;
using System.Text;
using Diplomski.RatingHub.Application.Enums;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Web.Components.Shared;
using Diplomski.RatingHub.Web.Constants;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using Diplomski.RatingHub.Web.Services.Interfaces;
using Diplomski.RatingHub.Web.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Radzen;
using Radzen.Blazor;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyDetailsPages;

public partial class CompanyDetails
{
    [Parameter] public int CompanyId { get; set; }
    
    [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public ICompanyDataService CompanyDataService { get; set; } = null!;
    [Inject] public ICategoryDataService CategoryDataService { get; set; } = null!;
    [Inject] public IReviewDataService ReviewDataService { get; set; } = null!;
    [Inject] public ICurrentUserService CurrentUserService { get; set; } = null!;
    
    private CompanyDetailsDto Company { get; set; } = new();
    private List<CategoryParentDto> _breadcrumbs = new List<CategoryParentDto>();

    private CurrentUserDto _currentUser;
    
    private const string _edit = "edit";
    private const string _delete = "delete";
    private string _trueDataPercentage = "0%";
    private string _falseDataPercentage = "0%";
    private bool _isDescriptionExpanded = false;
    private bool _hasMoreDescription;
    private string _shortDescription = "";
    public int _descriptionMaxLength = 220;

    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            var res = await InvokeDataServiceMethod(
                () => CompanyDataService.GetCompanyDetails(CompanyId), 
                errorMessage: "Greška pri učitavanju");
            if (!res.ExceptionOccurred)
            {
                Company = res.Result!;
                SetDescription();
                await LoadBreadcrumbs();
                CalculateTrueAndFalseDataPercentage();
            }
            else
            {
                Company = null;
            }
        }
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetCurrentUser();
        }
    }

    private async Task GetCurrentUser()
    {
        var currentUser = await CurrentUserService.GetCurrentUserAsync(JSRuntime);
        if (currentUser == null)
        {
            ShowNotification("Doslo je do greske prilikom ucitavanja korisnika", NotificationSeverity.Error);
            return;
        }
        _currentUser = currentUser;
        await InvokeAsync(StateHasChanged);
    }

    private async Task LoadBreadcrumbs()
    {
        var response = await InvokeDataServiceMethod(
            () => CategoryDataService.GetCategoryParents(Company.CategoryId),
            errorMessage: "Greška pri učitavanju roditeljskih kategorija");

        if (!response.ExceptionOccurred)
        {
            _breadcrumbs = response.Result.ToList();
            _breadcrumbs.Add(new CategoryParentDto{Id = 0, Name = Company.Name});
        }
    }

    private string FullAddress =>
        $"{Company.City}, {Company.Location}, {Company.Street} {Company.HouseNumber}";

    private string GetProfileImage()
    {
        return string.IsNullOrEmpty(Company.ProfileImagePath)
            ? "/images/companyImages/genericCompanyImage.svg"
            : Company.ProfileImagePath;
    }

    private void CalculateTrueAndFalseDataPercentage()
    {
        int falseDataNumber = Company.ReviewsCount - Company.CompanyDataTrueCount;
        
        if (Company.ReviewsCount > 0)
        {
            _falseDataPercentage = $"{((double)falseDataNumber/Company.ReviewsCount) * 100}%";
            _trueDataPercentage = $"{((double)Company.CompanyDataTrueCount/Company.ReviewsCount) * 100}%";
        }
    }

    private void SetDescription()
    {
        _hasMoreDescription = !string.IsNullOrWhiteSpace(Company.Description) && Company.Description.Length > _descriptionMaxLength;
        
        _shortDescription = _hasMoreDescription 
            ? Company.Description!.Substring(0, _descriptionMaxLength)
            : Company.Description ?? "";
    }

    private async Task GoToReview()
    {
        if (_currentUser is null)
        {
            ShowNotification("Doslo je do greske, molimo vas pokusajte kasnije", NotificationSeverity.Error);
            return;
        }
        
        if (_currentUser.IsAuthenticated && _currentUser.CurrentUserProfile!.Blocked)
        {
            ShowNotification("Vas profil je blokiran, zbog toga ne mozete oceniti kompaniju");
            return;
        }
        
        var result = await InvokeDataServiceMethod(
            () => ReviewDataService.GetIfReviewAlreadyExists(_currentUser.IndetityId, CompanyId),
            errorMessage: "Doslo je do greske, molimo vas pokusajte kasnije");

        if (result.ExceptionOccurred)
            return;

        if (result.Result)
        {
            ShowNotification("Vec ste ocenili ovog pružaoca usluga, nije dozvoljeno ocenjivati istog pružaoca usluga više puta", NotificationSeverity.Error);
            return;
        }
        
        var encodedIdentifier = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(_currentUser.IndetityId));
        NavigationManager.NavigateTo($"/companies/{CompanyId}/create-review?identifier={encodedIdentifier}&isAuthenticated={_currentUser.IsAuthenticated}");
    }
    
    private void OnBreadcrumbClicked(int categoryId)
    {
        NavigationManager.NavigateTo($"/companies?CityId={Company.CityId}&CategoryId={categoryId}", true);
    }

    public async Task ReportCompanyClicked()
    {
        var result = await DialogService.OpenAsync<ReportContentDialog>(
            "Prijavite pružaoca usluga",
            new Dictionary<string, object?>
            {
                { "ReportedEntityType", ReportedContentEntityType.Company },
                { "ReportedEntityId", Company.Id },
                { "ContentOwnerId", Company.OwnerId}
            },
            new DialogOptions
            {
                Width = "500px",
                Height = "auto",
                Style = "margin-top: 130px"
            });

        if (result is true)
        {
            ShowNotification("Uspesno ste prijavili pružaoca usluga", NotificationSeverity.Success);
        }
    }

    private async Task OnCompanyActionsClicked(RadzenProfileMenuItem item)
    {
        switch (item.Value)
        {
            case _edit:
                await EditCompanyClicked();
                break;
            case _delete:
                await DeleteCompanyClicked();
                break;
        }
    }

    private async Task EditCompanyClicked()
    {
        var result = await DialogService.OpenAsync<AnonymousEditContentDialog>(
            "Potvrda koda za azuriranje kompanije",
            new Dictionary<string, object?>
            {
                { "ContentType", AnonymousEditContentType.Company },
                { "IsEdit", true },
                { "Text", "Unesite kod od 15 karaktera koji ste dobili nakon kreiranja kompanije" },
                { "EntityId", Company.Id }
            },
            new DialogOptions
            {
                Width = "500px",
                Height = "auto",
                Style = "margin-top: 130px"
            });

        if (result is true)
        {
            NavigationManager.NavigateTo($"/companies/{Company.Id}/edit");
        }
    }

    private async Task DeleteCompanyClicked()
    {
        var result = await DialogService.OpenAsync<AnonymousEditContentDialog>(
            "Potvrda koda za brisanje kompanije",
            new Dictionary<string, object?>
            {
                { "ContentType", AnonymousEditContentType.Company },
                { "IsEdit", false },
                { "Text", "Unesite kod od 15 karaktera koji ste dobili nakon kreiranja kompanije" },
                { "EntityId", Company.Id }
            },
            new DialogOptions
            {
                Width = "500px",
                Height = "auto",
                Style = "margin-top: 130px"
            });

        if (result is true)
        {
            NavigationManager.NavigateTo($"/companies?CityId={Company.CityId}&CategoryId={Company.CategoryId}", true);
            ShowNotification("Uspesno ste izbrisali kompaniju", NotificationSeverity.Success);
        }
    }
    
    private string GetStarsFillStyle(double rating)
    {
        var percentage = (rating / 5.0) * 100;
        return $"--stars-fill: {percentage}%";
    }
    
    private async Task OpenGallery(int startIndex)
    {
        await DialogService.OpenAsync<ImageGalleryDialog>(
            "Galerija",
            new Dictionary<string, object?>
            {
                { "Images", Company.Images! },
                { "StartIndex", startIndex }
            },
            new DialogOptions
            {
                Width = "70%",
                Height = "70%",
                Style = "margin-top: 100px",
                CssClass = "image-gallery-dialog"
            });
    }
    
    private void ToggleDescription()
    {
        _isDescriptionExpanded = !_isDescriptionExpanded;
    }
}