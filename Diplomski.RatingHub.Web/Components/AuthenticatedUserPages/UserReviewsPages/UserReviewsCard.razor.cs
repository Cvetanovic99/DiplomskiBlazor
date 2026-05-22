using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Web.Components.Shared;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserReviewsPages;

public partial class UserReviewsCard
{
    [Parameter] public FilteredReviewDto Review { get; set; }
    [Parameter] public EventCallback OnDeleteReview { get; set; }
    
    [Inject] public IReviewDataService ReviewDataService { get; set; }
    
    private const string _edit = "edit";
    private const string _delete = "delete";

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
    
    
    private async Task OnReviewActionsClicked(RadzenProfileMenuItem item)
    {
        switch (item.Value)
        {
            case _edit:
                EditReviewClicked();
                break;
            case _delete:
                await DeleteReviewClicked();
                break;
        }
    }
    
    private void EditReviewClicked()
    {
        NavigationManager.NavigateTo($"/reviews/{Review.Id}/edit");
    }
    
    private async Task DeleteReviewClicked()
    {
        var res = await DialogService.Confirm("Da li ste sigurni da zelite da izbrisete ovu ocenu?","Brisanje ocene",
            new ConfirmOptions { OkButtonText = "Izbrisi", CancelButtonText = "Odustani", ShowClose = false });
        if (res is true)
        {
            var response = await InvokeDataServiceMethod(
                () => ReviewDataService.DeleteReview(Review.Id),
                errorMessage: "Doslo je do greske prilikom brisanja");

            if (response)
            {
                if (OnDeleteReview.HasDelegate)
                {
                    await OnDeleteReview.InvokeAsync();
                }
            }
        }
    }
    
    private async Task ReportCompanyResponseClicked()
    {
        var result = await DialogService.OpenAsync<ReportContentDialog>(
            "Prijavite odgovor kompanije",
            new Dictionary<string, object?>
            {
                { "ReportedEntityType", ReportedContentEntityType.CompanyResponse },
                { "ReportedEntityId", Review.CompanyResponse!.Id },
                { "ContentOwnerId", Review.CompanyResponse.CompanyOwnerId },
                { "ReviewId", Review.Id }
            },
            new DialogOptions
            {
                Width = "500px",
                Height = "auto",
                Style = "margin-top: 130px"
            });

        if (result is true)
        {
            ShowNotification("Uspesno ste prijavili odgovor kompanije", NotificationSeverity.Success);
        }
    }

    private string GetStarsFillStyle(double rating)
    {
        var percentage = (rating / 5.0) * 100;
        return $"--stars-fill: {percentage}%";
    }
}