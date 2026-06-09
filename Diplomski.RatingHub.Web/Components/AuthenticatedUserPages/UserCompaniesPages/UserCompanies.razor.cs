using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Web.Components.Shared;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using Diplomski.RatingHub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserCompaniesPages;

public partial class UserCompanies
{
    [Inject] public ICompanyDataService  CompanyDataService { get; set; } = null!;
    [Inject] public ICurrentUserService  CurrentUserService { get; set; } = null!;
    [Inject] public ICategoryDataService CategoryDataService { get; set; } = null!;
    [Inject] public ICompanyVerificationRequestDataService CompanyVerificationRequestDataService { get; set; } =  null!;
    
    private IEnumerable<UserCompanyDto> _companies = new List<UserCompanyDto>();
    private int _companiesTotalCount;
    private bool IsCompanyLoading;
    
    private AuthenticatedUserDto _authenticatedUser;
    private UserCompanyDto Company { get; set; } = new();
    private List<CategoryParentDto> _breadcrumbs = new List<CategoryParentDto>();
    
    private int _pageSize = 1;
    private int _currentPage;
    string pagingSummaryFormat = "Str. {0} od {1} (ukupno {2} kompanija)";
    private const string _edit = "edit";
    private const string _delete = "delete";
    private double _trueDataPercentage;
    private double _falseDataPercentage;
    private bool _isDescriptionExpanded = false;
    private bool _hasMoreDescription;
    private string _shortDescription = "";
    public int _descriptionMaxLength = 220;
    
    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
                await GetCurrentUser();
                await LoadCompanies();

                if (Company != null)//Everythig here depends on Company
                {
                    SetDescription();
                    await LoadBreadcrumbs();
                    CalculateTrueAndFalseDataPercentage();
                }
        }
    }
    
    private async Task LoadCompanies(int skip = 0)
    {
        if (skip == 0)
            _currentPage = 0;
        
        IsCompanyLoading = true;

        var res = await InvokeDataServiceMethod(
            () => CompanyDataService.GetUserCompanies(_authenticatedUser.UserProfileId,
                new QueryArgs { Skip = skip, Take = _pageSize, OrderBy = "Created desc"}),
            errorMessage: "Greška pri učitavanju");

        if (!res.ExceptionOccurred)
        {
            _companies = res.Result.Items;
            _companiesTotalCount = res.Result.TotalCount;
            Company = _companies.FirstOrDefault()!;
            StateHasChanged();
        }

        IsCompanyLoading = false;
    }
    
    private async Task GetCurrentUser()
    {
        var currentUser = await CurrentUserService.GetAuthenticatedUserAsync();
        if (currentUser == null)
        {
            ShowNotification("Doslo je do greske prilikom ucitavanja korisnika", NotificationSeverity.Error);
            return;
        }
        _authenticatedUser = currentUser;
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
    
    private void SetDescription()
    {
        _hasMoreDescription = !string.IsNullOrWhiteSpace(Company.Description) && Company.Description.Length > _descriptionMaxLength;
        
        _shortDescription = _hasMoreDescription 
            ? Company.Description!.Substring(0, _descriptionMaxLength)
            : Company.Description ?? "";
    }
    
    private void CalculateTrueAndFalseDataPercentage()
    {
        int falseDataNumber = Company.ReviewsCount - Company.CompanyDataTrueCount;
        
        if (Company.ReviewsCount > 0)
        {
            _falseDataPercentage = ((double)falseDataNumber/Company.ReviewsCount) * 100;
            _trueDataPercentage = ((double)Company.CompanyDataTrueCount/Company.ReviewsCount) * 100;
        }
    }
    
    private async Task OnPageChanged(PagerEventArgs args)
    {
        await LoadCompanies(args.Skip);
    }
    
    private string GetProfileImage()
    {
        return string.IsNullOrEmpty(Company.ProfileImagePath)
            ? "/images/companyImages/genericCompanyImage.svg"
            : Company.ProfileImagePath;
    }

    private void OnCreateCompanyClicked()
    {
        NavigationManager.NavigateTo($"/createCompany?OwnerId={_authenticatedUser.UserProfileId}");
    }
    
    private async Task OnClaimCompanyClicked()
    {
        var result = await DialogService.OpenAsync<ClaimCompanyDialog>(
            "Preuzimanje kompanije",
            new Dictionary<string, object?>
            {
                { "UserProfileId", _authenticatedUser.UserProfileId }
            },
            options: new DialogOptions
            {
                Width = "500px",
                Height = "auto",
                Style = "margin-top: 130px",
                ShowClose = false
            });

        if (result is true)
        {
            await LoadCompanies(0);
            ShowNotification("Uspesno ste preuzeli kompaniju", NotificationSeverity.Success);
        }
    }

    private async Task VerifyCompanyClicked()
    {
        var result = await DialogService.OpenAsync<RequestCompanyVerificationDialog>(
            "Zahtev za verifikaciju",
            new Dictionary<string, object?>
            {
                { "UserProfileId", _authenticatedUser.UserProfileId },
                { "CompanyId", Company.Id }
            },
            options: new DialogOptions
            {
                Width = "500px",
                Height = "auto",
                Style = "margin-top: 130px",
                ShowClose = false
            });

        if (result != null)
        {
            Company.VerificationRequest = (UserCompanyVerificationRequestDto)result;
            StateHasChanged();
            ShowNotification("Uspesno ste preuzeli kompaniju", NotificationSeverity.Success);
        }
        
    }
    
    private async Task SponsorCompanyClicked()
    {
        var res = await DialogService.Confirm("Cena sponzorisanja kompanije je 10€. Nakon uplate vasa kompanija ce se prikazivati na vrhu pretrage i bice dodatno istaknuta korisnicima.",
            "Sponzorisanje kompanije", new ConfirmOptions { OkButtonText = "Nastavi", CancelButtonText = "Odustani", ShowClose = false });
        if (res is true)
        {
            var response = await InvokeDataServiceMethod(
                () => CompanyDataService.CreateCheckoutSession(Company.Id),
                errorMessage: "Doslo je do greske, molimo vas pokusajte kasnije.");

            if (!response.ExceptionOccurred)
            {
                NavigationManager.NavigateTo(response.Result, true);
            }
        }
        
    }
    
    private async Task RemoveCompanyFromSponsoredClicked()
    {
        var res = await DialogService.Confirm("Da li ste sigurni da zelite da prestanete sa sponzorstvom kompanije?","Prekid sponzorstva kompanije",
            new ConfirmOptions { OkButtonText = "Prestani", CancelButtonText = "Odustani", ShowClose = false });
        if (res is true)
        {
            var response = await InvokeDataServiceMethod(
                () => CompanyDataService.RemoveCompanyFromSponsored(Company.Id),
                errorMessage: "Doslo je do greske");

            if (response)
            {
                ShowNotification("Uspesno ste prestali sa sponzorstvom", NotificationSeverity.Success);
                Company.IsSponsored = false;
                Company.SponsoredUntil = null;
                StateHasChanged();
            }
        }
    }

    private async Task DeleteVerificationRequestClicked()
    {
        var res = await DialogService.Confirm("Da li ste sigurni da zelite da izbrisete zahtev za verifikaciju","Brisanje verifikacionog zahteva",
            new ConfirmOptions { OkButtonText = "Izbrisi", CancelButtonText = "Odustani", ShowClose = false });
        if (res is true)
        {
            var response = await InvokeDataServiceMethod(
                () => CompanyVerificationRequestDataService.DeleteVerificationRequest(Company.VerificationRequest!.Id),
                errorMessage: "Doslo je do greske prilikom brisanja");

            if (response)
            {
                ShowNotification("Uspesno ste izbrisali zahtev", NotificationSeverity.Success);
                Company.VerificationRequest = null;
                StateHasChanged();
            }
        }
    }

    private async Task OnCompanyActionsClicked(RadzenProfileMenuItem item)
    {
        switch (item.Value)
        {
            case _edit:
                EditCompanyClicked();
                break;
            case _delete:
                await DeleteCompanyClicked();
                break;
        }
    }
    
    private void EditCompanyClicked()
    {
        NavigationManager.NavigateTo($"/companies/{Company.Id}/edit");
    }

    private async Task DeleteCompanyClicked()
    {
        var res = await DialogService.Confirm("Da li ste sigurni da zelite da izbrisete ovu kompaniju","Brisanje kompanije",
            new ConfirmOptions { OkButtonText = "Izbrisi", CancelButtonText = "Odustani", ShowClose = false });
        if (res is true)
        {
            var response = await InvokeDataServiceMethod(
                () => CompanyDataService.DeleteCompanyAsOwner(Company.Id),
                errorMessage: "Doslo je do greske prilikom brisanja");

            if (response)
            {
                ShowNotification("Uspesno ste izbrisali kompaniju", NotificationSeverity.Success);
                await LoadCompanies(0);
            }
        }
    }
    
    private string GetStarsFillStyle(double rating)
    {
        var percentage = (rating / 5.0) * 100;
        return $"--stars-fill: {percentage}%";
    }
    
    private string FullAddress =>
        $"{Company.City}, {Company.Location}, {Company.Street} {Company.HouseNumber}";
    
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
    
    private string? GetSponsoredDate()
    {
        if (Company.SponsoredUntil == null)
        {
            return "";
        }
        else
        {
            TimeZoneInfo serbiaZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            DateTime serbiaTime = TimeZoneInfo.ConvertTimeFromUtc(Company.SponsoredUntil.Value, serbiaZone);
            
            return serbiaTime.ToString("MMMM dd, yyyy HH:mm", new System.Globalization.CultureInfo("sr-Latn-RS"));
        }
    }
}