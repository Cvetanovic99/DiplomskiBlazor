using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyDetailsPages;

public partial class CompanyDetailsAdditionalData 
{
    [Parameter] public int CompanyId { get; set; }
    
    [Inject] public ICompanyDataService CompanyDataService { get; set; } = null!;

    private CompanyDetailsAdditionalDataDto? Model;
    private List<StarItem> _starBreakdown = new();
    private double _verifiedPercentage;
    private double _unverifiedPercentage;

    private bool _isJsInitialized;

    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            var res = await InvokeDataServiceMethod(
                () => CompanyDataService.GetCompanyDetailsAdditionalData(CompanyId), 
                errorMessage: "Greška pri učitavanju");
            if (!res.ExceptionOccurred)
            {
                Model = res.Result;
                CalculateStarProcentage();
                CalculateVerifiedUnverifiedPercentage();
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Model == null) return;
        if (!_isJsInitialized)
        {
            _isJsInitialized = true;
            await InitializeMap();
        }
    }

    private void CalculateStarProcentage()
    {
        _starBreakdown = new List<StarItem>
        {
            CreateStarItem(5, Model!.FiveStarReviewsCount, "#16a34a"),
            CreateStarItem(4, Model.FourStarReviewsCount, "#84cc16"),
            CreateStarItem(3, Model.ThreeStarReviewsCount, "#eab308"),
            CreateStarItem(2, Model.TwoStarReviewsCount, "#f97316"),
            CreateStarItem(1, Model.OneStarReviewsCount, "#ef4444"),
        };
    }

    private void CalculateVerifiedUnverifiedPercentage()
    {
        _verifiedPercentage = Model!.ReviewsCount == 0
            ? 0
            : double.Round(((double)Model.VerifiedReviewsCount / Model.ReviewsCount) * 100);
        
        _unverifiedPercentage = Model!.ReviewsCount == 0
            ? 0
            : 100 - _verifiedPercentage;
    }

    private StarItem CreateStarItem(int stars, int count, string color)
    {
        double percentage = Model!.ReviewsCount == 0
            ? 0
            : ((double)count / Model.ReviewsCount) * 100;

        return new StarItem
        {
            Stars = stars,
            Percentage = double.Round(percentage),
            Color = color
        };
    }

    private async Task InitializeMap()
    {
        await JSRuntime.InvokeVoidAsync("companyDetailsMap.init",
            Model!.Latitude,
            Model!.Longitude);
    }

    private string GetStarsFillStyle(double rating)
    {
        var percentage = (rating / 5.0) * 100;
        return $"--stars-fill: {percentage}%";
    }
    
    private string GetAggregateFillColor(double value)
    {
        if (value >= 4.5) return "#16a34a"; // green
        if (value >= 3.5) return "#84cc16"; // light green
        if (value >= 2.5) return "#eab308"; // yellow
        if (value >= 1.5) return "#f97316"; // orange
        return "#ef4444"; // red
    }

    private class StarItem
    {
        public int Stars { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; } = "";
    }
}