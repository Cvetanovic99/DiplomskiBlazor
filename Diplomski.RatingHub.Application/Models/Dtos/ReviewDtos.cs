namespace Diplomski.RatingHub.Application.Models.Dtos;

public class CreateReviewDto
{
    public string Comment { get; set; }
    public string? ReviewerFullName { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; }
    public int? CompanyOwnerId { get; set; }
    public string ReviewerIdentifier { get; set; }
    public bool IsAuthenticated { get; set; }
    public bool IsCompanyDataTrue { get; set; }

    public IList<ReviewGradesDto>  ReviewGrades { get; set; } = new List<ReviewGradesDto>();
    public IList<CreateReviewImageDto> Images { get; set; } = new List<CreateReviewImageDto>();
}

public class ReviewGradesDto
{
    public int Grade { get; set; }
    public int RatingCriterionId { get; set; }
    public string RatingCriterionName { get; set; }
}