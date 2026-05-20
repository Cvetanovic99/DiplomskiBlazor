using AutoMapper;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Reviews.Queries;

public class GetReviewForEditQuery : IRequest<EditReviewDto>
{
    public int ReviewId { get; set; }
}

public class GetReviewForEditQueryValidator : AbstractValidator<GetReviewForEditQuery>
{
    public GetReviewForEditQueryValidator()
    {
        RuleFor(x => x.ReviewId).GreaterThan(0);
    }
}

public class GetReviewForEditQueryHandler : IRequestHandler<GetReviewForEditQuery, EditReviewDto>
{
    private readonly IDatabaseRepository<Review> _reviewRepository;

    public GetReviewForEditQueryHandler(IDatabaseRepository<Review> reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<EditReviewDto> Handle(GetReviewForEditQuery request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetSingleAndProject<EditReviewDto>(new Specification<Review>(r =>  r.Id == request.ReviewId));
        if (review == null)
            throw new AppException("Ocena ne postoji");
        
        return review;
    }
}

public class EditReviewDto : IMapFrom<Review>
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public int?  ReviewerId { get; set; }
    public string? ReviewerFullName { get; set; }
    public bool IsAnonymousReview { get; set; }
    public bool IsCompanyDataTrue { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = null!;
    public string? CompanyProfileImagePath { get; set; }
    public ICollection<EditReviewImageDto> Images { get; set; } = new List<EditReviewImageDto>();
    public ICollection<EditReviewGradeDto> Grades { get; set; } = new List<EditReviewGradeDto>();
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Review, EditReviewDto>()
            .ForMember(dest => dest.CompanyName,
                opt => opt.MapFrom(src => src.Company.Name))
            .ForMember(dest => dest.CompanyProfileImagePath,
                opt => opt.MapFrom(src => 
                    src.Company.Images.FirstOrDefault(i => i.IsProfile).Path));
    }
}

public class EditReviewImageDto : IMapFrom<ReviewImage>
{
    public required string Title { get; set; }
    public required string Path { get; set; }
}

public class EditReviewGradeDto : IMapFrom<ReviewGrade>
{
    public int Id { get; set; }
    public int Grade { get; set; }
    public int RatingCriterionId { get; set; }
    public string RatingCriterionName { get; set; } = null!;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ReviewGrade, EditReviewGradeDto>()
            .ForMember(dest => dest.RatingCriterionName,
                opt => opt.MapFrom(src => src.RatingCriterion.Name))
            .ForMember(dest => dest.RatingCriterionId,
                opt => opt.MapFrom(src => src.RatingCriterion.Id));
    }
}