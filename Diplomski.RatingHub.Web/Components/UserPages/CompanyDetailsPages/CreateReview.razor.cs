using System.Text;
using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using Diplomski.RatingHub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyDetailsPages;

public partial class CreateReview
{
    [Parameter] public int CompanyId { get; set; }

    [SupplyParameterFromQuery] public string? Identifier { get; set; }
    [SupplyParameterFromQuery] public bool? IsAuthenticated { get; set; }

    [Inject] public IReviewDataService ReviewDataService { get; set; } = null!;
    [Inject] public ICompanyDataService CompanyDataService { get; set; } = null!;
    [Inject] public IHttpService HttpService { get; set; }

    private CreateReviewDto Model = new();
    private CompanyWithRatingCriteriaDto Company;

    private List<CreateReviewImageDto> _images = new();

    private List<object> verifyOptions = new()
    {
        new { Text = "Nisu tačni", Value = false },
        new { Text = "Tačni su", Value = true },
    };
    public const string _reviewImageUrl = "review-image";
    private const string _commentPlaceholder = "Šta vam se svidelo ili šta vam se nije svidelo? Šta ova kompanija radi dobro, " +
                                               "ili kako može da se poboljša? Ne zaboravite da budete iskreni, od pomoći i konstruktivni!";

    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {

            if (Identifier is null || IsAuthenticated is null)
            {
                ShowNotification("URL nije ispravan pokusajte ponovo ceo proces");
                return;
            }

            var res = await InvokeDataServiceMethod(
                () => CompanyDataService.GetCompanyWithRatingCriteria(CompanyId),
                errorMessage:"Doslo je do greske");

            if (!res.ExceptionOccurred)
            {
                Company = res.Result;

                Model.CompanyId = CompanyId;
                Model.ReviewerIdentifier = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Identifier));
                Model.IsAuthenticated = IsAuthenticated.Value;
                Model.CompanyOwnerId = Company!.OwnerId;

                Model.ReviewGrades = Company.RatingCriteria
                    .Select(x => new ReviewGradesDto
                    {
                        RatingCriterionId = x.Id,
                        RatingCriterionName = x.Name,
                        Grade = 0
                    }).ToList();
            }
        }
    }

    private async Task HandleFiles(InputFileChangeEventArgs e)
    {
        if(_images.Count + e.FileCount > 5)
        {
            ShowNotification("Mozete uploadovati maksimalno 5 slika", NotificationSeverity.Error);
            return;
        }

        var files = e.GetMultipleFiles(10);
        
        foreach (var file in files)
        {
            var content = ProcessImage(file);
            if (content.ExceptionOccured)
            {
                ShowNotification($"Slika: {file.Name} je prevelika, maksimalna velicina je '500 KB.'", NotificationSeverity.Error);
                continue;
            }
            
            var response = await HttpService.UploadImage(content.Content!, _reviewImageUrl);
            if(response.ExceptionOccurred)    
            {
                ShowNotification("Greška pri uploadu slike " + file.Name, NotificationSeverity.Error);
                continue;
            }
            
            _images.Add(new CreateReviewImageDto
            {
                Path = response.Result!.Path,
                Title = response.Result!.Title,
            });
        }
        
    }
    
    private ProcessImageResponse ProcessImage(IBrowserFile file)
    {
        var response = new ProcessImageResponse();
        try
        {
            var content = new MultipartFormDataContent();
            var stream = file.OpenReadStream(5_000_000);
            content.Add(new StreamContent(stream), "file", file.Name);
            
            response.Content = content;
            response.ExceptionOccured = false;
        }
        catch (Exception e)
        {
            response.Content = null;
            response.ExceptionOccured = true;
        }
        
        return response;
    }

    private async Task RemoveImage(CreateReviewImageDto img)
    {
        var success = await HttpService.DeleteImage(_reviewImageUrl, img.Path);

        if (success)
            _images.Remove(img);
        else
            ShowNotification("Greška pri brisanju slike: " + img.Path, NotificationSeverity.Error);
    }

    private async Task HandleSubmit(CreateReviewDto model)
    {
        Model.Images = _images;
        
        var result = await InvokeDataServiceMethod(
            () => ReviewDataService.CreateReview(Model),
            errorMessage:"Doslo je do greske prilikom ocenjivanja, molimo vas pokusajte kasnije");
        
        if (!result.ExceptionOccurred)
        {
            if (IsAuthenticated!.Value)
            {
                NavigationManager.NavigateTo($"/user/reviews");
            }
            else
            {
                var res = await DialogService.Alert(GetEditAnonymousIdentifierText(result.Result!), 
                    "Kod za azuriranje ocene",
                    new AlertOptions { OkButtonText = "Prikazi ocenu", ShowClose = false });
                if(res is true)
                    NavigationManager.NavigateTo($"/companies/{CompanyId}");
            }
        }
    }

    private async Task Cancel()
    {
        foreach (var img in _images)
            await HttpService.DeleteImage(_reviewImageUrl, img.Path);

        NavigationManager.NavigateTo($"/companies/{CompanyId}");
    }

    private string GetCriterionCompnentName(int id)
    {
        return $"rating_{id}";
    }
}
