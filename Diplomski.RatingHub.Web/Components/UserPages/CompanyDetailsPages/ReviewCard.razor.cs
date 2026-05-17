using Diplomski.RatingHub.Application.Enums;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Web.Components.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyDetailsPages;

public partial class ReviewCard 
{
    [Parameter] public FilteredReviewDto Review { get; set; }
    [Parameter] public EventCallback OnReviewDelete { get; set; }
    
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
        return Review.Created.ToString("MMMM dd, yyyy");
    }
    
    
    private async Task OnReviewActionsClicked(RadzenProfileMenuItem item)
    {
        switch (item.Value)
        {
            case _edit:
                await EditReviewClicked();
                break;
            case _delete:
                await DeleteReviewClicked();
                break;
        }
    }
    
    private async Task EditReviewClicked()
    {
        var result = await DialogService.OpenAsync<AnonymousEditContentDialog>(
            "Potvrda koda za azuriranje ocene",
            new Dictionary<string, object?>
            {
                { "ContentType", AnonymousEditContentType.Review },
                { "IsEdit", true },
                { "Text", "Unesite kod od 15 karaktera koji ste dobili nakon kreiranja ocene" },
                { "EntityId", Review.Id }
            },
            new DialogOptions
            {
                Width = "500px",
                Height = "auto",
                Style = "margin-top: 130px"
            });

        if (result is true)
        {
            NavigationManager.NavigateTo($"/reviews/{Review.Id}/edit");
        }
    }
    
    private async Task DeleteReviewClicked()
    {
        var result = await DialogService.OpenAsync<AnonymousEditContentDialog>(
            "Potvrda koda za brisanje ocene",
            new Dictionary<string, object?>
            {
                { "ContentType", AnonymousEditContentType.Review },
                { "IsEdit", false },
                { "Text", "Unesite kod od 15 karaktera koji ste dobili nakon kreiranja ocene" },
                { "EntityId", Review.Id }
            },
            new DialogOptions
            {
                Width = "500px",
                Height = "auto",
                Style = "margin-top: 130px"
            });

        if (result is true)
        {
            if (OnReviewDelete.HasDelegate)
            {
                await OnReviewDelete.InvokeAsync();
            }
        }
    }
    
    public async Task ReportReviewClicked()
    {
        var result = await DialogService.OpenAsync<ReportContentDialog>(
            "Prijavite ocenjivanja",
            new Dictionary<string, object?>
            {
                { "ReportedEntityType", ReportedContentEntityType.Review },
                { "ReportedEntityId", Review.Id },
                { "ContentOwnerId", Review.ReviewerId}
            },
            new DialogOptions
            {
                Width = "500px",
                Height = "auto",
                Style = "margin-top: 130px"
            });

        if (result is true)
        {
            ShowNotification("Uspesno ste prijavili ocenjivanje", NotificationSeverity.Success);
        }
    }
    
    public async Task ReportCompanyResponseClicked()
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