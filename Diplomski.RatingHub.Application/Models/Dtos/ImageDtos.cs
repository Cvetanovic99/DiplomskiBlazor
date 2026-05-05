namespace Diplomski.RatingHub.Web.Models;

public class UploadImageResponseDto
{
    public required string Path { get; set; }
    public required string Title { get; set; }
}

public class CreateImageDto
{
    public required string Path { get; set; }
    public required string Title { get; set; }
    public bool IsProfile { get; set; }
}