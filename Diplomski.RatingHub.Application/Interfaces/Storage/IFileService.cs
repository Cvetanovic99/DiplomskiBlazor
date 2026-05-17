using Diplomski.RatingHub.Application.Models.Dtos;
using Microsoft.AspNetCore.Http;

namespace Diplomski.RatingHub.Application.Interfaces.Storage;

public interface IFileService
{ 
    Task<UploadImageResponseDto> UploadImageAsync(IFormFile? file, string folderName);
    void DeleteImage(string fileName);
}