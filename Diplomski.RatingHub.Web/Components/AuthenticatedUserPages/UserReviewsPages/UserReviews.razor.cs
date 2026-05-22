using Diplomski.RatingHub.Application.Enums;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using Diplomski.RatingHub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserReviewsPages;

public partial class UserReviews
{
    [Parameter] public int CompanyId { get; set; }
    
    [Inject] public IReviewDataService ReviewDataService { get; set; } = null!;
    [Inject] public ICurrentUserService  CurrentUserService { get; set; } = null!;
    
    private AuthenticatedUserDto _authenticatedUser;
    
    private IEnumerable<FilteredReviewDto> _reviews = new List<FilteredReviewDto>();
    private int _totalCount;
    private int _pageSize = 15;
    private int _currentPage;
    private int _skipPages;
    

    private UserReviewsFilterModel _filter = new();
    private IEnumerable<ReviewSortingOptions> _sortingOptions = Enum.GetValues<ReviewSortingOptions>();
    private ReviewSortingOptions _sortingOption = ReviewSortingOptions.CreatedDesc;

    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            await GetCurrentUser();
            _filter.UserProfileId = _authenticatedUser.UserProfileId;

            await LoadReviews();
        }
    }

    private async Task LoadReviews(int skip = 0)
    {
        if (skip == 0)
            _currentPage = 0;
        
        _skipPages = skip;
        _filter.QueryArgs = new QueryArgs { Skip = skip, Take = _pageSize, OrderBy = GetOrderBy() };
        
        var result = await InvokeDataServiceMethod(
            () => ReviewDataService.GetUserReviews(_filter),
            errorMessage: "Greška pri učitavanju ocena");

        if (!result.ExceptionOccurred)
        {
            _reviews = result.Result.Items;
            _totalCount = result.Result.TotalCount;
        }
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

    private async Task OnPageChanged(PagerEventArgs args)
    {
        await LoadReviews(args.Skip);
    }

    private async Task OnSearchKeyPress(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await LoadReviews();
        }
    }

    private async Task RatingFilterChanged()
    {
        await LoadReviews();
    }

    private async Task OnCheckboxFilterChanged(bool isChecked)
    {
        await LoadReviews();
    }

    private async Task OnDeleteReviewClicked()
    {
        ShowNotification("Uspesno ste izbrisali ocenu", NotificationSeverity.Success);
        await LoadReviews(_skipPages);
    }

    private string GetOrderBy()
    {
        string value = "";
        switch (_sortingOption)
        {
            case ReviewSortingOptions.CreatedDesc:
                value = $"{nameof(FilteredReviewDto.Created)} desc";
                break;
            case ReviewSortingOptions.CreatedAsc:
                value = $"{nameof(FilteredReviewDto.Created)} asc";
                break;
            case ReviewSortingOptions.RatingValueDesc:
                value = $"{nameof(FilteredReviewDto.OverallScore)} desc";
                break;
            case ReviewSortingOptions.RatingValueAsc:
                value = $"{nameof(FilteredReviewDto.OverallScore)} asc";
                break;
            default:
                break;
        }

        return value;
    }
    
    private string GetSortingOptions(object value)
    {
        var option = (ReviewSortingOptions)value;
        switch (option)
        {
            case ReviewSortingOptions.CreatedDesc:
                return "Najnovije";
            case ReviewSortingOptions.CreatedAsc: 
                return "Starije";
            case ReviewSortingOptions.RatingValueDesc:
                return "Ocene opadajuce";
            case ReviewSortingOptions.RatingValueAsc:
                return "Ocene rastuce";
            default:
                return "";
        }
    }
}

public class UserReviewsFilterModel
{
    public int UserProfileId { get; set; }
    public string FilterValue { get; set; }
    public QueryArgs QueryArgs { get; set; } //For orderBy and pagination
    public int MinOverallScore { get; set; }
    public bool OnlyConfirmed { get; set; }
    public bool OnlyWithCompanyResponse { get; set; }
}