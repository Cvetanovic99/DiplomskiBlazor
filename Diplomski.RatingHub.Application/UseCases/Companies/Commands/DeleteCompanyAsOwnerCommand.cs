using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Interfaces.Storage;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Commands;

public class DeleteCompanyAsOwnerCommand : IRequest<Unit>
{
    public int CompanyId { get; set; }
}

public class DeleteCompanyAsOwnerCommandValidator : AbstractValidator<DeleteCompanyAsOwnerCommand>
{
    public DeleteCompanyAsOwnerCommandValidator()
    {
        RuleFor(x => x.CompanyId).GreaterThan(0).WithMessage("CompanyId je obavezan");
    }
}

public class DeleteCompanyAsOwnerCommandHandler : IRequestHandler<DeleteCompanyAsOwnerCommand, Unit>
{
    private readonly IDatabaseRepository<Company> _companyRepository;
    private readonly IDatabaseRepository<CompanyRatingAggregate> _companyRatingAggregateRepository;
    private readonly IDatabaseRepository<CompanyResponse> _companyResponseRepository;
    private readonly IDatabaseRepository<Review> _reviewsRepository;
    private readonly IDatabaseRepository<CompanyVerificationRequest> _verificationRequestsRepository;
    private readonly IDatabaseRepository<CompanyImage> _imagesRepository;
    private readonly IDatabaseRepository<CompanyResponseImage> _companyResponseImageRepository;
    private readonly IDatabaseRepository<ReviewImage> _reviewImageRepository;
    private readonly IDatabaseRepository<Like> _likesRepository;
    private readonly IDatabaseRepository<ReviewGrade> _reviewGradesRepository;
    private readonly IFileService _fileService;

    public DeleteCompanyAsOwnerCommandHandler(
        IDatabaseRepository<Company> companyRepository,
        IDatabaseRepository<CompanyRatingAggregate> companyRatingAggregateRepository,
        IDatabaseRepository<CompanyResponse> companyResponseRepository,
        IDatabaseRepository<Review> reviewsRepository,
        IDatabaseRepository<CompanyVerificationRequest> verificationRequestsRepository,
        IDatabaseRepository<CompanyImage> imagesRepository,
        IDatabaseRepository<CompanyResponseImage> companyResponseImageRepository,
        IDatabaseRepository<ReviewImage> reviewImageRepository,
        IDatabaseRepository<Like> likesRepository,
        IDatabaseRepository<ReviewGrade> reviewGradesRepository,
        IFileService fileService)
    {
        _companyRepository = companyRepository;
        _companyRatingAggregateRepository = companyRatingAggregateRepository;
        _companyResponseRepository = companyResponseRepository;
        _reviewsRepository = reviewsRepository;
        _verificationRequestsRepository = verificationRequestsRepository;
        _imagesRepository = imagesRepository;
        _companyResponseImageRepository = companyResponseImageRepository;
        _reviewImageRepository = reviewImageRepository;
        _likesRepository = likesRepository;
        _reviewGradesRepository = reviewGradesRepository;
        _fileService = fileService;
    }

    public async Task<Unit> Handle(DeleteCompanyAsOwnerCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetSingleBySpec(new Specification<Company>(c => c.Id == request.CompanyId)
            .AddInclude(c => c.CompanyRatingAggregates)
            .AddInclude("Responses.Images")
            .AddInclude(c => c.VerificationRequests)
            .AddInclude(c => c.Images));
        if (company is null)
            throw new AppException("Kompanija ne postoji");

        var companyReviews = await _reviewsRepository.Get(new Specification<Review>(r => r.CompanyId == request.CompanyId)
            .AddInclude(r=> r.Images)
            .AddInclude(r => r.Likes)
            .AddInclude(r => r.Grades));
        if (company.IsAnonymousCreator && companyReviews.Any())//Owner can't delete company because it was not created by him
        {
            company.OwnerId = null;
            company.IsVerified = false;
            await _companyRepository.Update(company);
            
            return Unit.Value;
        }


        //Delete CompanyRatingAggregates
        if(company.CompanyRatingAggregates.Any())
            await _companyRatingAggregateRepository.DeleteRange(company.CompanyRatingAggregates);

        
        //Delete CompanyResponses
        List<CompanyResponseImage> companyResponseImages = new List<CompanyResponseImage>();
        foreach (var response in company.Responses)
        {
            companyResponseImages.AddRange(response.Images);
        }

        foreach (var companyResponseImage in companyResponseImages)
        {
            _fileService.DeleteImage(companyResponseImage.Path);
        }
        await _companyResponseImageRepository.DeleteRange(companyResponseImages);
        await _companyResponseRepository.DeleteRange(company.Responses);
        
        
        //Delete Reviews
        List<ReviewImage> reviewImages = new List<ReviewImage>();
        List<Like> reviewLikes = new List<Like>();
        List<ReviewGrade> reviewGrades = new List<ReviewGrade>();
        foreach (var review in companyReviews)
        {
            reviewImages.AddRange(review.Images);
            reviewLikes.AddRange(review.Likes);
            reviewGrades.AddRange(review.Grades);
        }

        foreach (var image in reviewImages)
        {
            _fileService.DeleteImage(image.Path);
        }
        await _reviewImageRepository.DeleteRange(reviewImages);
        await _likesRepository.DeleteRange(reviewLikes);
        await _reviewGradesRepository.DeleteRange(reviewGrades);
        await _reviewsRepository.DeleteRange(companyReviews);
        
        //Delete VerificationRequests
        if(company.VerificationRequests.Any())
            await _verificationRequestsRepository.DeleteRange(company.VerificationRequests);
        
        //DeleteCompanyImages
        foreach (var image in company.Images)
        {
            _fileService.DeleteImage(image.Path);
        }
        await _imagesRepository.DeleteRange(company.Images);

        await _companyRepository.Delete(company);

        return Unit.Value;
    }
}