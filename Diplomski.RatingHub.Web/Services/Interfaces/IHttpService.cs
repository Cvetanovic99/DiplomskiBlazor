using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Web.Models;

namespace Diplomski.RatingHub.Web.Services.Interfaces;

public interface IHttpService
{
    Task<HttpResponseDto<UploadImageResponseDto>> UploadImage(MultipartFormDataContent content, string url);
    Task<bool> DeleteImage(string url, string imagePath);
}