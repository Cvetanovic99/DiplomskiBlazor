using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Application.UseCases.Companies.Commands;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Reviews.Commands;

public class CreateReviewCommand : IRequest<Unit>
{
    public required string Comment { get; set; }
    public string? ReviewerFullName { get; set; }
    public bool IsAnonymousReview { get; set; }
    public string? AnonymousEditIdentifier { get; set; }
    public bool IsCompanyDataTrue { get; set; }
    public required string ReviewerIdentifier { get; set; }
    public int CompanyId { get; set; }
    public IList<CreateReviewImageDto> Images { get; set; } =  new List<CreateReviewImageDto>();
    public IList<ReviewGradesDto>  ReviewGrades { get; set; } = new List<ReviewGradesDto>();
}

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.Comment).NotEmpty();
        RuleFor(x => x.CompanyId).GreaterThan(0);
        RuleFor(x => x.ReviewerIdentifier).NotEmpty();
    }
}

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Unit>
{
    private readonly IDatabaseRepository<Review> _reviewRepository;
    private readonly IDatabaseRepository<UserProfile> _userProfileRepository;
    private readonly IDatabaseRepository<Company> _companyRepository;

    public CreateReviewCommandHandler(IDatabaseRepository<Review> reviewRepository,
        IDatabaseRepository<UserProfile> userProfileRepository,
        IDatabaseRepository<Company> companyRepository)
    {
        _reviewRepository = reviewRepository;
        _userProfileRepository = userProfileRepository;
        _companyRepository = companyRepository;
    }

    public async Task<Unit> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        int? reviewerId = null;
        if (!request.IsAnonymousReview)
        {
            var userProfile = await _userProfileRepository.GetSingleBySpec(
                new Specification<UserProfile>(u => u.IdentityUserId == request.ReviewerIdentifier));
            if (userProfile == null)
                throw new AppException("korisnik ne postoji");
            
            reviewerId =  userProfile.Id;
        }
        
        double overallScore = request.ReviewGrades.Sum(rg => rg.Grade) / (double)request.ReviewGrades.Count;
        
        IList<ReviewGrade> grades = request.ReviewGrades
            .Select(rg => new ReviewGrade { Grade = rg.Grade, RatingCriterionId = rg.RatingCriterionId }).ToList();
        
        IList<ReviewImage> images = request.Images.Select(i => new ReviewImage{Title = i.Title, Path = i.Path}).ToList();
        
        var company = await _companyRepository.GetSingleBySpec(new Specification<Company>(c => c.Id == request.CompanyId)
            .AddInclude(c => c.CompanyRatingAggregates));
        if (company == null)
            throw new AppException("Kompanija ne postoji");
        
        //Update CompanyRatingAggregate
        foreach (var grade in request.ReviewGrades)
        {
            var ratingAggregate = 
                company.CompanyRatingAggregates.FirstOrDefault(a => a.RatingCriterionId == grade.RatingCriterionId);
            ratingAggregate!.SumValue += grade.Grade;
            ratingAggregate.RatingsCount += 1;
            ratingAggregate.AverageValue = (double)ratingAggregate.SumValue / ratingAggregate.RatingsCount;
        }
        
        //Update Company
        company.SumGradesValue += overallScore;
        company.ReviewsCount += 1;
        company.OverallAverageGrade = company.SumGradesValue / company.ReviewsCount;
        
        await _companyRepository.Update(company);

        var review = new Review
        {
            Comment = request.Comment,
            ReviewerFullName = request.ReviewerFullName,
            OverallScore = overallScore,
            IsAnonymousReview = request.IsAnonymousReview,
            AnonymousEditIdentifier = request.AnonymousEditIdentifier,
            IsCompanyDataTrue = request.IsCompanyDataTrue,
            ReviewerIdentifier = request.ReviewerIdentifier,
            ReviewerId = reviewerId,
            CompanyId = request.CompanyId,
            Grades = grades,
            Images = images
        };
        
        await _reviewRepository.Insert(review);
        
        return Unit.Value;
    }
}