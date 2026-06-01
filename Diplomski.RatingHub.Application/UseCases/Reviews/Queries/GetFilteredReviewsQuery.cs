using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;
using Diplomski.RatingHub.Application.Specifications;

namespace Diplomski.RatingHub.Application.UseCases.Reviews.Queries;

public class GetFilteredReviewsQuery : IRequest<IPaginatedList<FilteredReviewDto>>
{
    public int CompanId { get; set; }
    public string FilterValue { get; set; }
    public QueryArgs QueryArgs { get; set; } //For orderBy and pagination
    public double MinOverallScore { get; set; }
    public bool OnlyConfirmed { get; set; }
    public bool OnlyWithCompanyResponse { get; set; }
    public int? CurrentAuthenticatedUserId { get; set; }
}

public class GetFilteredReviewsQueryValidator : AbstractValidator<GetFilteredReviewsQuery>
{
    public GetFilteredReviewsQueryValidator()
    {
        RuleFor(x => x.CompanId).GreaterThan(0)
            .WithMessage("CompanId mora biti veci od 0");

        RuleFor(x => x.FilterValue).MaximumLength(100)
            .WithMessage("FilterValue ne sme biti duzi od 100 karaktera");
    }
}

public class GetFilteredReviewsQueryHandler : IRequestHandler<GetFilteredReviewsQuery, IPaginatedList<FilteredReviewDto>>
{
    private readonly IDatabaseRepository<Review> _reviewsRepository;
    private readonly IDatabaseRepository<Like> _likesRepository;

    public GetFilteredReviewsQueryHandler(
        IDatabaseRepository<Review> reviewsRepository,
        IDatabaseRepository<Like> likesRepository)
    {
        _reviewsRepository = reviewsRepository;
        _likesRepository = likesRepository;
    }

    public async Task<IPaginatedList<FilteredReviewDto>> Handle(GetFilteredReviewsQuery request, CancellationToken cancellationToken)
    {
        var specification = new Specification<Review>(r => r.CompanyId == request.CompanId);
        
        if (!string.IsNullOrWhiteSpace(request.FilterValue))
        {
            specification.And(r => r.Comment.Contains(request.FilterValue));
        }
        
        // Filter by OverallScore
        if (request.MinOverallScore > 0)
        {
            specification.And(r => r.OverallScore >= request.MinOverallScore);
        }

        if (request.OnlyConfirmed)
        {
            specification.And(r => r.ReviewerId != null);
        }
        
        if (request.OnlyWithCompanyResponse)
        {
            specification.And(r => r.CompanyResponse != null);
        }
        
        var reviews = await _reviewsRepository.GetAndProjectAsPaginatedList<FilteredReviewDto>(specification, request.QueryArgs);

        if (request.CurrentAuthenticatedUserId is not null)//Show to user what he already liked
        {
            IList<int> reviewIdsWithLikes = reviews.Items.Where(r => r.LikesCount > 0).Select(r => r.Id).ToList();
            var likes = await _likesRepository.Get(
                new Specification<Like>(l => reviewIdsWithLikes.Contains(l.ReviewId) && l.UserId == request.CurrentAuthenticatedUserId));

            foreach (var like in likes)
            {
                reviews.Items.First(r => r.Id == like.ReviewId).IsLikedByCurrentAuthenticatedUser = true;
            }
        }

        return reviews;
    }
}

public class FilteredReviewDto : IMapFrom<Review>
{
    public int Id { get; set; }
    public string Comment { get; set; } = null!;
    public double OverallScore { get; set; }
    public bool IsAnonymousReview { get; set; }
    public string? ReviewerFullName { get; set; }
    public int LikesCount { get; set; }
    public bool IsLikedByCurrentAuthenticatedUser { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = null!;
    public int?  ReviewerId { get; set; }
    public ReviewerDto? Reviewer { get; set; }
    public CompanyResponseDto? CompanyResponse { get; set; }
    
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
    public List<string>? Images { get; set; } = new();
    public ICollection<ReviewGradeDto> Grades { get; set; } = new List<ReviewGradeDto>();
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Review, FilteredReviewDto>()
            .ForMember(dest => dest.Images,
                opt => opt.MapFrom(src => src.Images.Select(i => i.Path).ToList()))
            .ForMember(dest => dest.LikesCount,
                opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.CompanyName,
                opt => opt.MapFrom(src => src.Company.Name));
    }
}

public class ReviewerDto : IMapFrom<UserProfile>
{
    public string FullName { get; set; }
    public string? ProfileImage { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<UserProfile, ReviewerDto>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.Name + " " + src.Surname))
            .ForMember(dest => dest.ProfileImage,
                opt => opt.MapFrom(src => src.ProfileImage.Path));
    }
}

public class CompanyResponseDto : IMapFrom<CompanyResponse>
{
    public int Id { get; set; }
    public string CompanyName { get; set; } 
    public int CompanyOwnerId { get; set; } 
    public int ReviewId { get; set; }
    public string Text { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
    public string? ProfileImage { get; set; }
    public List<string> Images { get; set; } = new();
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<CompanyResponse, CompanyResponseDto>()
            .ForMember(dest => dest.Images,
                opt => opt.MapFrom(src => src.Images.Select(i => i.Path).ToList()))
            .ForMember(dest => dest.ProfileImage,
                opt => opt.MapFrom(src =>
                    src.Company.Images.Where(i => i.IsProfile).Select(i => i.Path).FirstOrDefault()))
        .ForMember(dest => dest.CompanyName,
            opt => opt.MapFrom(src => src.Company.Name))
        .ForMember(dest => dest.CompanyOwnerId,
            opt => opt.MapFrom(src => src.Company.OwnerId));
    }
}

public class ReviewGradeDto : IMapFrom<ReviewGrade>
{
    public int Grade { get; set; }
    public string CriterionName { get; set; } 
    public int SortOrder { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<ReviewGrade, ReviewGradeDto>()
            .ForMember(dest => dest.CriterionName,
                opt => opt.MapFrom(src => src.RatingCriterion.Name))
            .ForMember(dest => dest.SortOrder,
                opt => opt.MapFrom(src => src.RatingCriterion.SortOrder));
    }
}