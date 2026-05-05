using Diplomski.RatingHub.Web.Models;
using Diplomski.RatingHub.Web.Services.Interfaces;

namespace Diplomski.RatingHub.Web.Services;

public class HttpService(HttpClient httpClient): IHttpService
{
    private const string _uploadImagePath = "upload/image/";
    public async Task<HttpResponseDto<UploadImageResponseDto>> UploadImage(MultipartFormDataContent content, string url)
    {
        var response = new HttpResponseDto<UploadImageResponseDto>();

        try
        {
            var res = await httpClient.PostAsync($"{_uploadImagePath}{url}", content);

            if (!res.IsSuccessStatusCode)
            {
                response.ExceptionOccurred = true;
            }
            else
            {
                response.Result = await res.Content.ReadFromJsonAsync<UploadImageResponseDto>();
            }
        }
        catch (Exception e)
        {
           response.ExceptionOccurred = true;
        }
        
        return response;
    }

    public async Task<bool> DeleteImage(string url, string imagePath)
    {
        try
        {
            var res = await httpClient.DeleteAsync($"{_uploadImagePath}{url}?path={imagePath}");
            if (!res.IsSuccessStatusCode)
            {
                return false;
            }
        }
        catch (Exception e)
        {
            return false;
        }

        return true;
    }
}