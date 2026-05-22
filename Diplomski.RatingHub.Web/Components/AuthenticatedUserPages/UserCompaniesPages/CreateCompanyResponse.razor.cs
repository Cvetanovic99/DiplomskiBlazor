using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using Diplomski.RatingHub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserCompaniesPages;

public partial class CreateCompanyResponse
{
    [Parameter] public int CompanyId { get; set; }
    [Parameter] public int ReviewId { get; set; }
    [Parameter] public int ReviewerId { get; set; }
    [Parameter] public string CompanyName { get; set; }
    
    [Inject] public ICompanyResponseDataService CompanyResponseDataService { get; set; } = null!;
    [Inject] public IHttpService HttpService { get; set; }

    private CreateCompanyResponseDto Model = new();

    private List<CreateReviewImageDto> _images = new();
    
    private const string _responseImageUrl = "response-image";
    

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
            
            var response = await HttpService.UploadImage(content.Content!, _responseImageUrl);
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
        var success = await HttpService.DeleteImage(_responseImageUrl, img.Path);

        if (success)
            _images.Remove(img);
        else
            ShowNotification("Greška pri brisanju slike: " + img.Path, NotificationSeverity.Error);
    }

    private async Task HandleSubmit(CreateCompanyResponseDto model)
    {
        Model.CompanyId = CompanyId;
        Model.ReviewId = ReviewId;
        Model.Images = _images;
        Model.ReviewOwnerId = ReviewerId;
        Model.CompanyName = CompanyName;
        
        var result = await InvokeDataServiceMethod(
            () => CompanyResponseDataService.CreateCompanyResponse(Model),
            errorMessage:"Doslo je do greske prilikom kreiranja odgovora");
        
        if (!result.ExceptionOccurred)
        {
            DialogService.Close(result.Result);
        }
    }

    private async Task Cancel()
    {
        foreach (var img in _images)
            await HttpService.DeleteImage(_responseImageUrl, img.Path);

        DialogService.Close(null);
    }
}