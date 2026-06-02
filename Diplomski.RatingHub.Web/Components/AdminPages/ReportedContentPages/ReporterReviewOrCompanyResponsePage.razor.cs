using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Web.Components.Shared;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.AdminPages.ReportedContentPages;

public partial class ReporterReviewOrCompanyResponsePage 
{
    [Parameter] public int ReviewId { get; set; }
    
    [Inject] public IReviewDataService ReviewDataService { get; set; } 
    
    private FilteredReviewDto Review;

    protected override async Task OnInitializedAsync()
    {
        var res = await InvokeDataServiceMethod(
            () => ReviewDataService.GetReviewForAdmin(ReviewId), 
            errorMessage: "Greška pri učitavanju");
        
        if (!res.ExceptionOccurred)
        {
            Review = res.Result!;
        }
    }
    
    private string GetReviewerName()
    {
        if (Review.ReviewerId != null)
            return Review.Reviewer?.FullName ?? "Korisnik";

        if (!string.IsNullOrEmpty(Review.ReviewerFullName))
            return Review.ReviewerFullName;

        return "Anonimni Korisnik";
    }
    
    private bool IsConfirmed() => Review.ReviewerId != null;
    
    private string GetProfileImage()
    {
        if (Review.ReviewerId == null)
            return "/images/userProfileImages/universalProfileImage.svg";

        if (!string.IsNullOrEmpty(Review.Reviewer?.ProfileImage))
            return Review.Reviewer.ProfileImage;

        return "/images/userProfileImages/universalProfileImage.svg";
    }
    
    private string GetCompanyImage()
    {
        if (!string.IsNullOrEmpty(Review.CompanyResponse?.ProfileImage))
            return Review.CompanyResponse.ProfileImage;

        return "/images/companyImages/genericCompanyImage.svg";
    }
    
    private async Task OpenGallery(int index, List<string> images)
    {
        await DialogService.OpenAsync<ImageGalleryDialog>(
            "Galerija",
            new Dictionary<string, object?>
            {
                { "Images", images! },
                { "StartIndex", index }
            },
            new DialogOptions
            {
                Width = "70%",
                Height = "70%",
                Style = "margin-top: 100px",
                CssClass = "image-gallery-dialog"
            });
    }
    
    private string GetDate()
    {
        return Review.Created.ToString("MMMM dd, yyyy", new System.Globalization.CultureInfo("sr-Latn-RS"));
    }
    
    private string GetStarsFillStyle(double rating)
    {
        var percentage = (rating / 5.0) * 100;
        return $"--stars-fill: {percentage}%";
    }
}