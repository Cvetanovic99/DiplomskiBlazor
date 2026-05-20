using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Reviews.Commands;

public class EditReviewCommand : IRequest<Unit>
{
    public int ReviewId { get; set; }
    public required string Comment { get; set; }
    public string? ReviewerFullName { get; set; }
    public bool IsCompanyDataTrue { get; set; }
    public IList<EditReviewImageDto> Images { get; set; } =  new List<EditReviewImageDto>();
    public IList<EditReviewGradeDto>  ReviewGrades { get; set; } = new List<EditReviewGradeDto>();
}

public class EditReviewCommandValidator : AbstractValidator<EditReviewCommand>
{
    public EditReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId).GreaterThan(0);
        RuleFor(x => x.Comment).NotEmpty();
    }
}

public class EditReviewCommandHandler : IRequestHandler<EditReviewCommand, Unit>
{
    private readonly IDatabaseRepository<Review>  _reviewRepository;
    private readonly IDatabaseRepository<Company>  _companyRepository;
    private readonly IDatabaseRepository<ReviewImage>  _reviewImagesRepository;

    public EditReviewCommandHandler(IDatabaseRepository<Review> reviewRepository,
        IDatabaseRepository<Company> companyRepository,
        IDatabaseRepository<ReviewImage> reviewImagesRepository)
    {
        _reviewRepository = reviewRepository;
        _companyRepository = companyRepository;
        _reviewImagesRepository = reviewImagesRepository;
    }

    public async Task<Unit> Handle(EditReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetSingleBySpec(new Specification<Review>(r => r.Id == request.ReviewId)
            .AddInclude(r => r.Grades)
            .AddInclude(r => r.Images));
        if (review == null)
            throw new AppException("Ocena ne postoji");

        var company = await _companyRepository.GetSingleBySpec(new Specification<Company>(c => c.Id == review.CompanyId)
            .AddInclude(c => c.CompanyRatingAggregates));
        if(company == null)
            throw new AppException("Kompanija ne postoji");
        
        //Update rating aggregate - remove old values
        foreach (var grade in review.Grades)
        {
            var ratingAggregate =
                company.CompanyRatingAggregates.FirstOrDefault(a => a.RatingCriterionId == grade.RatingCriterionId);
            ratingAggregate!.SumValue -= grade.Grade;
            ratingAggregate.RatingsCount -= 1;
            ratingAggregate.AverageValue = ratingAggregate!.RatingsCount == 0
                ? 0
                : (double)ratingAggregate.SumValue / ratingAggregate.RatingsCount;
        }
        
        //Update company - remove old values
        company.SumGradesValue -= review.OverallScore;
        company.ReviewsCount -= 1;
        company.OverallAverageGrade = company.ReviewsCount == 0
            ? 0
            : company.SumGradesValue / company.ReviewsCount;
        
        
        //New values
        double overallScore = request.ReviewGrades.Sum(rg => rg.Grade) / (double)request.ReviewGrades.Count;
        
        IList<ReviewImage> images = request.Images.Select(i => new ReviewImage{Title = i.Title, Path = i.Path}).ToList();
        
        //Update rating aggregate - set new values
        foreach (var grade in request.ReviewGrades)
        {
            var ratingAggregate = 
                company.CompanyRatingAggregates.FirstOrDefault(a => a.RatingCriterionId == grade.RatingCriterionId);
            ratingAggregate!.SumValue += grade.Grade;
            ratingAggregate.RatingsCount += 1;
            ratingAggregate.AverageValue = (double)ratingAggregate.SumValue / ratingAggregate.RatingsCount;
        }
        
        //Update company - set new values
        company.SumGradesValue += overallScore;
        company.ReviewsCount += 1;
        company.OverallAverageGrade = company.SumGradesValue / company.ReviewsCount;
        
        await _companyRepository.Update(company);
        
        //Update review
        review.Comment = request.Comment;
        review.ReviewerFullName = request.ReviewerFullName;
        review.OverallScore = overallScore;
        review.IsCompanyDataTrue = request.IsCompanyDataTrue;

        //Update review grades
        foreach (var grade in request.ReviewGrades)
        {
            review.Grades.First(g => g.Id == grade.Id).Grade =  grade.Grade;
        }
       
        //Update review images
        await _reviewImagesRepository.DeleteRange(review.Images);
        review.Images = images;
        
        await _reviewRepository.Update(review);
        
        return Unit.Value;
    }
}