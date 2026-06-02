using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using Diplomski.RatingHub.Web.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.AuthenticatedUserPages.UserCompaniesPages;

public partial class EditCompanyResponse
{
    [Parameter] public EditCompanyResponseDto Model { get; set; }
    
    [Inject] public ICompanyResponseDataService CompanyResponseDataService { get; set; } = null!;
    [Inject] public IHttpService HttpService { get; set; } = null!;
    
    
    private List<EditReviewImageDto> _existingImages = new();
    private List<string> _imagesToDelete = new();
    private List<CreateReviewImageDto> _newImages = new();
    
    private List<object> verifyOptions = new()
    {
        new { Text = "Nisu tačni", Value = false },
        new { Text = "Tačni su", Value = true },
    };
    
    private const string _responseImageUrl = "response-image";
    
    private bool IsLoadingSubmit;

    protected override void OnParametersSet()
    {
        _existingImages = Model.Images.ToList();
    }

    private async Task HandleSubmit(EditCompanyResponseDto model)
    {
        IsLoadingSubmit = true;

        model.Images = _existingImages
            .Select(x => new EditReviewImageDto
            {
                Path = x.Path,
                Title = x.Title
            })
            .Concat(_newImages.Select(x => new EditReviewImageDto
            {
                Path = x.Path,
                Title = x.Title
            }))
            .ToList();

        var result = await InvokeDataServiceMethod(
            () => CompanyResponseDataService.EditCompanyResponse(model),
            errorMessage: "Greška pri izmeni");

        IsLoadingSubmit = false;
        if (!result.ExceptionOccurred)
        {
            foreach (var path in _imagesToDelete)
                await HttpService.DeleteImage(_responseImageUrl, path);

            DialogService.Close(result.Result);
        }
    }
    
    private async Task HandleFiles(InputFileChangeEventArgs e)
    {
        if (_existingImages.Count + _newImages.Count + e.FileCount > 5)
        {
            ShowNotification("Mozete dodati maksimalno 5 slika", NotificationSeverity.Error);
            return;
        }

        var files = e.GetMultipleFiles(10);

        foreach (var file in files)
        {
            var content = ProcessImage(file);
            if (content.ExceptionOccured)
            {
                ShowNotification($"Slika: {file.Name} je prevelika, maksimalna velicina je '10MB.'", NotificationSeverity.Error);
                continue;
            }

            var response = await HttpService.UploadImage(content.Content!, _responseImageUrl);
            if (response.ExceptionOccurred)
            {
                ShowNotification("Greška pri uploadu slike", NotificationSeverity.Error);
                continue;
            }

            _newImages.Add(new CreateReviewImageDto
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
            var stream = file.OpenReadStream(10_000_000);
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
    
    private void RemoveExistingImage(EditReviewImageDto img)
    {
        _existingImages.Remove(img);
        _imagesToDelete.Add(img.Path);
    }

    private async Task RemoveNewImage(CreateReviewImageDto img)
    {
        var success = await HttpService.DeleteImage(_responseImageUrl, img.Path);

        if (success)
            _newImages.Remove(img);
    }
    
    private async Task Cancel()
    {
        foreach (var img in _newImages)
        {
            await HttpService.DeleteImage(_responseImageUrl, img.Path);
        }

       DialogService.Close(null);
    }
}