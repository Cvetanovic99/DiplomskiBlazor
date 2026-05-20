using Diplomski.RatingHub.Application.Enums;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserCompaniesPages;

public partial class UserCompanyReviews
{
    [Parameter] public int CompanyId { get; set; }
    
    [Inject] public IReviewDataService ReviewDataService { get; set; } = null!;

    private IEnumerable<FilteredReviewDto> _reviews = new List<FilteredReviewDto>();
    private int _totalCount;
    private int _pageSize = 10;
    private int _currentPage;
    private int _skipPages;


    private UserCompanyReviewsFilterModel _filter = new();
    private IEnumerable<ReviewSortingOptions> _sortingOptions = Enum.GetValues<ReviewSortingOptions>();
    private ReviewSortingOptions _sortingOption = ReviewSortingOptions.CreatedDesc;

    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            _filter.CompanId = CompanyId;

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
            () => ReviewDataService.GetUserCompanyReviews(_filter),
            errorMessage: "Greška pri učitavanju ocena");

        if (!result.ExceptionOccurred)
        {
            _reviews = result.Result.Items;
            _totalCount = result.Result.TotalCount;
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

    public class UserCompanyReviewsFilterModel
    {
        public int CompanId { get; set; }
        public string FilterValue { get; set; }
        public QueryArgs QueryArgs { get; set; } //For orderBy and pagination
        public int MinOverallScore { get; set; }
        public bool OnlyConfirmed { get; set; }
        public bool OnlyWithoutCompanyResponse { get; set; }
    }
}