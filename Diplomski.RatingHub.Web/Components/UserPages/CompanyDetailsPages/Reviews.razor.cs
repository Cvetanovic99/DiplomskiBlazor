using System.Security.Claims;
using Diplomski.RatingHub.Application.Enums;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Web.Constants;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Data.Services;
using Diplomski.RatingHub.Web.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyDetailsPages;

public partial class Reviews
{
    [Parameter] public int CompanyId { get; set; }

    [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; }
    [Inject] public IReviewDataService ReviewDataService { get; set; } = null!;
    
    private IEnumerable<FilteredReviewDto> _reviews = new List<FilteredReviewDto>();
    private int _totalCount;
    private int _pageSize = 10;
    private int _currentPage;
    private int _skipPages;
    

    private ReviewsFilterModel _filter = new();
    private IEnumerable<ReviewSortingOptions> _sortingOptions = Enum.GetValues<ReviewSortingOptions>();
    private ReviewSortingOptions _sortingOption = ReviewSortingOptions.CreatedDesc;

    protected override async Task OnInitializedAsync()
    {
        _filter.CompanId = CompanyId;

        await LoadReviews();
    }

    private async Task LoadReviews(int skip = 0)
    {
        if (skip == 0)
            _currentPage = 0;
        
        _skipPages = skip;
        _filter.QueryArgs = new QueryArgs { Skip = skip, Take = _pageSize, OrderBy = GetOrderBy() };
        
        var result = await InvokeDataServiceMethod(
            () => ReviewDataService.GetFilteredReviews(_filter),
            errorMessage: "Greška pri učitavanju ocena");

        if (!result.ExceptionOccurred)
        {
            //_reviews = result.Result.Items;
            //_totalCount = result.Result.TotalCount;
            _reviews = new List<FilteredReviewDto>{ new FilteredReviewDto
            {
                Id = 1,
                Comment = "Svidjala mi se saradnja sa ovim pruzaocem usluga, sve je bilo korektno kako smo se dogovorili. Jedino mislim da je mogao da zavrsi brze jer se puno oduzilo, sta da kazem jos, volim vas.",
                OverallScore = 3.54,
                IsAnonymousReview = false,
                ReviewerFullName = "",
                LikesCount = 354,
                ReviewerId = null,
                Reviewer = new ReviewerDto
                {
                    FullName = "Goran Cvetanovic",
                    ProfileImage = "/images/companyImages/DSC_0323.jpg"
                },
                CompanyResponseId = 3,
                CompanyResponse = new CompanyResponseDto
                {
                    Id=3,
                    CompanyName = "Sabali programiranje",
                    Text = "Hvala na lepim komentarima gospodine, nadam se da cemo uvek ovako lepo saradjivati. Kada god treba nazovite za slicne radove i preporucite nas drugome.",
                    Created =  DateTime.Now,
                    Modified =  DateTime.Now.AddMonths(1),
                    ProfileImage = "/images/companyImages/DSC_0326.jpg",
                    Images = new List<string> {"/images/companyImages/DSC_0326.jpg", "/images/companyImages/0872fcc3-044f-4ca0-a1a2-a17133a8e3bf.jpg", 
                        "/images/companyImages/DSC_0326.jpg", "/images/companyImages/89995aec-289c-4a84-a60a-767ab57a2fee.jpg", "/images/companyImages/DSC_0326.jpg"}
                    
                },
                Created = DateTime.Today,
                Images = new List<string> {"/images/companyImages/DSC_0326.jpg", "/images/companyImages/0872fcc3-044f-4ca0-a1a2-a17133a8e3bf.jpg", 
                    "/images/companyImages/DSC_0326.jpg", "/images/companyImages/89995aec-289c-4a84-a60a-767ab57a2fee.jpg", "/images/companyImages/DSC_0326.jpg"},
                Grades = new List<ReviewGradeDto>{new ReviewGradeDto{CriterionName = "Cena", SortOrder = 1, Grade = 3},
                new ReviewGradeDto{CriterionName = "Usluga", SortOrder = 2, Grade = 4}, new ReviewGradeDto{CriterionName = "Vreme cekanja", SortOrder = 3, Grade = 5},}
            }};
            _totalCount = 30;
        }
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
    
    private async Task OnReviewDelete()
    {
        await LoadReviews(_skipPages);
        ShowNotification("Uspesno ste izbrisali ocenu", NotificationSeverity.Success);
    }

    private async Task GoToCreateReview()
    {
        string? userIdentityIdentifier = await HandleIdentityId();

        if (userIdentityIdentifier == null)
        {
            ShowNotification("Doslo je do greske, molimo vas pokusajte kasnije", NotificationSeverity.Error);
            return;
        }
        
        var result = await InvokeDataServiceMethod(
            () => ReviewDataService.GetIfReviewAlreadyExists(userIdentityIdentifier, CompanyId),
            errorMessage: "Doslo je do greske, molimo vas pokusajte kasnije");

        if (result.ExceptionOccurred)
            return;

        if (result.Result)
        {
            ShowNotification("Vec ste ocenili ovog pružaoca usluga, nije dozvoljeno ocenjivati istog pružaoca usluga više puta", NotificationSeverity.Error);
            return;
        }
        

        NavigationManager.NavigateTo($"/companies/{CompanyId}/create-review");
    }

    private async Task<string?> HandleIdentityId()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        
        if (user.Identity?.IsAuthenticated == true)
        {
            string identityId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(identityId))
                return identityId;
            else
                return null;
        }
        else
        {
            string? customGuid = await JSRuntime.GetItemFromLocalStorage(LocalStorageKeys.AnonymousUserCustomGuidKey);
            if (string.IsNullOrEmpty(customGuid))
            {
                string guid = Guid.NewGuid().ToString();
                bool res = await JSRuntime.SetItemToLocalStorage(LocalStorageKeys.AnonymousUserCustomGuidKey, guid);
                if (!res)
                {
                    return null;
                }
                
                return guid;
            }
            
            return customGuid;
        }
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

public class ReviewsFilterModel
{
    public int CompanId { get; set; }
    public string FilterValue { get; set; }
    public QueryArgs QueryArgs { get; set; } //For orderBy and pagination
    public int MinOverallScore { get; set; }
    public bool OnlyConfirmed { get; set; }
    public bool OnlyWithCompanyResponse { get; set; }
}