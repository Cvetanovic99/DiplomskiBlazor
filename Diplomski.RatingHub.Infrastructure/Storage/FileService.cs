using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Storage;
using Diplomski.RatingHub.Web.Models;
using Microsoft.AspNetCore.Http;

namespace Diplomski.RatingHub.Infrastructure.Storage;

public class FileService : IFileService
{
    private readonly IReadOnlyList<string> _allowedTypes =  new[] { ".jpg", ".jpeg", ".png", ".webp" };
    private const string BaseStorageFolder = "wwwroot";
    private const string ImagesStorageFolder = "images";
    
    
    public async Task<UploadImageResponseDto> UploadImageAsync(IFormFile? file, string folderName)
    {
        if (file is null || file.Length == 0)
            throw new AppException("Fajl nije validan");
        
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!_allowedTypes.Contains(extension))
            throw new AppException("Nepodržan format slike");

        if (file.Length > 5 * 1024 * 1024)
            throw new AppException("Maksimalna veličina je 5MB");

        var fileName = $"{Guid.NewGuid()}{extension}";
        var folder = Path.Combine(BaseStorageFolder,ImagesStorageFolder, folderName);

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var fullPath = Path.Combine(folder, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        var relativePath = $"/{ImagesStorageFolder}/{folderName}/{fileName}";

        return new UploadImageResponseDto
        {
            Path = relativePath,
            Title = file.FileName
        };
    }

    public void DeleteImage(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new AppException("Putanja nije validna");
        
        if (!path.StartsWith("/images"))
            throw new AppException("Putanja nije validna");
        

        var fullPath = Path.Combine(BaseStorageFolder, path.TrimStart('/'));

        if (!File.Exists(fullPath))
            throw new AppException("Fajl ne postoji");

        File.Delete(fullPath);
    }
}