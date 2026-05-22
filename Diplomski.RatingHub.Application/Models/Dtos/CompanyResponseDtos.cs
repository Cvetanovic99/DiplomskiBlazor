using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;

namespace Diplomski.RatingHub.Application.Models.Dtos;

public class CreateCompanyResponseDto
{
    public string Text { get; set; }
    
    public int CompanyId { get; set; }
    public string CompanyName { get; set; }
    public int ReviewId { get; set; }
    public int? ReviewOwnerId { get; set; }
    public IList<CreateReviewImageDto> Images { get; set; } =  new List<CreateReviewImageDto>();
}

public class EditCompanyResponseDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public IList<EditReviewImageDto> Images { get; set; } =  new List<EditReviewImageDto>();
}