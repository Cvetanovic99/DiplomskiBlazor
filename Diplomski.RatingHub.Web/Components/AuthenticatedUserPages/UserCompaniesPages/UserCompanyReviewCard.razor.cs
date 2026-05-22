using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Web.Components.Shared;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserCompaniesPages;

public partial class UserCompanyReviewCard 
{
    [Parameter] public FilteredReviewDto Review { get; set; }
    
    [Inject] public IReviewDataService ReviewDataService { get; set; } = null!;
    [Inject] public ICompanyResponseDataService CompanyResponseDataService { get; set; } = null!;
    
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
                await EditResponseClicked();
                break;
            case _delete:
                await DeleteResponseClicked();
                break;
        }
    }
    
    private async Task EditResponseClicked()
    {
        var result = await DialogService.OpenAsync<EditCompanyResponse>(
            "Azuriraj odgovor",
            new Dictionary<string, object?>
            {
                { "Model", new EditCompanyResponseDto
                {
                    Id = Review.CompanyResponse!.Id,
                    Text = Review.CompanyResponse.Text,
                    Images = Review.CompanyResponse.Images.Select(i => new EditReviewImageDto { Path = i, Title = "" }).ToList()
                } }
            },
            new DialogOptions
            {
                Width = "50%",
                Height = "70%",
                Style = "margin-top: 130px"
            });
        
        if (result is CompanyResponseDto dto)
        {
            Review.CompanyResponse = dto;
            ShowNotification("Uspesno ste azurirali odgovor", NotificationSeverity.Success);
            StateHasChanged();
        }
    }
    
    private async Task DeleteResponseClicked()
    {
        var res = await DialogService.Confirm("Da li ste sigurni da zelite da izbrisete odgovor?","Brisanje odgovora",
            new ConfirmOptions { OkButtonText = "Izbrisi", CancelButtonText = "Odustani", ShowClose = false });
        if (res is true)
        {
            var response = await InvokeDataServiceMethod(
                () => CompanyResponseDataService.DeleteCompanyResponse(Review.CompanyResponse!.Id),
                errorMessage: "Doslo je do greske prilikom brisanja");

            if (response)
            {
                ShowNotification("Uspesno ste izbrisali odgovor", NotificationSeverity.Success);
                Review.CompanyResponse = null;
                StateHasChanged();
            }
        }
    }
    
    public async Task AddResponseClicked()
    {
        var result = await DialogService.OpenAsync<CreateCompanyResponse>(
            "Dodajte odgovor",
            new Dictionary<string, object?>
            {
                { "CompanyId", Review.CompanyId },
                { "ReviewId", Review.Id },
                { "ReviewerId", Review.ReviewerId },
                { "CompanyName", Review.CompanyName }
            },
            new DialogOptions
            {
                Width = "50%",
                Height = "70%",
                Style = "margin-top: 130px"
            });
        
        if (result is CompanyResponseDto dto)
        {
            Review.CompanyResponse = dto;
            ShowNotification("Uspesno ste dodali odgovor", NotificationSeverity.Success);
            StateHasChanged();
        }
    }

    private string GetStarsFillStyle(double rating)
    {
        var percentage = (rating / 5.0) * 100;
        return $"--stars-fill: {percentage}%";
    }
}