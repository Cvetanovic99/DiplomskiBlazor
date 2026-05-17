using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Interfaces.Storage;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Reviews.Commands;

public class DeleteReviewCommand : IRequest<Unit>
{
    public int ReviewId { get; set; }
}

public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId).GreaterThan(0).WithMessage("ReviewId je obavezan");
    }
}

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Unit>
{
    private readonly IDatabaseRepository<Review> _reviewsRepository;
    private readonly IDatabaseRepository<ReviewImage> _reviewImagesRepository;
    private readonly IDatabaseRepository<CompanyResponse> _companyResponsesRepository;
    private readonly IDatabaseRepository<CompanyResponseImage> _companyResponseImagesRepository;
    private readonly IDatabaseRepository<Like> _likesRepository;
    private readonly IDatabaseRepository<CompanyRatingAggregate> _companyRatingAggregatesRepository;
    private readonly IDatabaseRepository<ReviewGrade> _reviewGradesRepository;
    private readonly IDatabaseRepository<Company> _companyRepository;
    private readonly IFileService _fileService;

    public DeleteReviewCommandHandler(
        IDatabaseRepository<Review> reviewsRepository,
        IDatabaseRepository<ReviewImage> reviewImagesRepository,
        IDatabaseRepository<CompanyResponse> companyResponsesRepository,
        IDatabaseRepository<CompanyResponseImage> companyResponseImagesRepository,
        IDatabaseRepository<Like> likesRepository,
        IDatabaseRepository<CompanyRatingAggregate> companyRatingAggregatesRepository,
        IDatabaseRepository<ReviewGrade> reviewGradesRepository,
        IDatabaseRepository<Company> companyRepository,
        IFileService fileService)
    {
        _reviewsRepository = reviewsRepository;
        _reviewImagesRepository = reviewImagesRepository;
        _companyResponsesRepository = companyResponsesRepository;
        _companyResponseImagesRepository = companyResponseImagesRepository;
        _likesRepository = likesRepository;
        _companyRatingAggregatesRepository = companyRatingAggregatesRepository;
        _reviewGradesRepository = reviewGradesRepository;
        _companyRepository = companyRepository;
        _fileService = fileService;
        
    }

    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewsRepository.GetSingleBySpec(new Specification<Review>(r => r.Id == request.ReviewId)
            .AddInclude(r => r.Images)
            .AddInclude(r => r.Likes)
            .AddInclude(r => r.Grades)
            .AddInclude(r => r.Company));

        if (review is null)
            throw new AppException("Ocena ne postoji");
        
        //Delete Images
        foreach (var image in review.Images)
        {
            _fileService.DeleteImage(image.Path);
        }
        await _reviewImagesRepository.DeleteRange(review.Images);

        //Delete CompanyResponse
        var companyResponse =
            await _companyResponsesRepository.GetSingleBySpec(new Specification<CompanyResponse>(r => r.ReviewId == review.Id)
                    .AddInclude(r => r.Images));
        if (companyResponse is not null)
        {
            foreach (var image in companyResponse.Images)
            {
                _fileService.DeleteImage(image.Path);
            }
            await _companyResponseImagesRepository.DeleteRange(companyResponse.Images);
            await _companyResponsesRepository.Delete(companyResponse);
        }
        
        //Delete Likes
        if(review.Likes.Any())
            await _likesRepository.DeleteRange(review.Likes);
        
        
        //Delete grades
       var companiesRatingAggregates = await _companyRatingAggregatesRepository.Get(
           new Specification<CompanyRatingAggregate>(a => a.CompanyId == review.CompanyId));
        foreach (var grade in review.Grades)
        {
            //Update rating aggregate
            var ratingAggregate =
                companiesRatingAggregates.FirstOrDefault(a => a.RatingCriterionId == grade.RatingCriterionId);
            ratingAggregate!.SumValue -= grade.Grade;
            ratingAggregate.RatingsCount -= 1;
            ratingAggregate.AverageValue = ratingAggregate!.RatingsCount == 0
                ? 0
                : (double)ratingAggregate.SumValue / ratingAggregate.RatingsCount;
        }
        await _companyRatingAggregatesRepository.UpdateRange(companiesRatingAggregates);
        await _reviewGradesRepository.DeleteRange(review.Grades);
        
        
        //Update company
        var company = review.Company;
        company.SumGradesValue -= review.OverallScore;
        company.ReviewsCount -= 1;
        company.OverallAverageGrade = company.ReviewsCount == 0
            ? 0
            : company.SumGradesValue / company.ReviewsCount;
        await _companyRepository.Update(company);
        
        await _reviewsRepository.Delete(review);
        
        return Unit.Value;
    }
}