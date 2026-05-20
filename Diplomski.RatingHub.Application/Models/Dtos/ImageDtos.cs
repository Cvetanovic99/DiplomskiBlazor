namespace Diplomski.RatingHub.Application.Models.Dtos;

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

public class CreateReviewImageDto
{
    public required string Path { get; set; }
    public required string Title { get; set; }
}