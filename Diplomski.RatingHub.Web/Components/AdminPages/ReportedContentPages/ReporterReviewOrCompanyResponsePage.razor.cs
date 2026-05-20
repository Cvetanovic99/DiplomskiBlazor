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
            //Review = res.Result!;
            Review = new FilteredReviewDto
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
            };
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
        return Review.Created.ToString("MMMM dd, yyyy");
    }
    
    private string GetStarsFillStyle(double rating)
    {
        var percentage = (rating / 5.0) * 100;
        return $"--stars-fill: {percentage}%";
    }
}