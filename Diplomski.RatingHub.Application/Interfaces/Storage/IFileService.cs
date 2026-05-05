using Diplomski.RatingHub.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Diplomski.RatingHub.Application.Interfaces.Storage;

public interface IFileService
{ 
    Task<UploadImageResponseDto> UploadImageAsync(IFormFile? file, string folderName);
    void DeleteImage(string fileName);
}