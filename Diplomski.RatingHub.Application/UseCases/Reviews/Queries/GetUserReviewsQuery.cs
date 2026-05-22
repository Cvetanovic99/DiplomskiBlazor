using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Reviews.Queries;

public class GetUserReviewsQuery : IRequest<IPaginatedList<FilteredReviewDto>>
{
    public int UserProfileId { get; set; }
    public string FilterValue { get; set; }
    public QueryArgs QueryArgs { get; set; } //For orderBy and pagination
    public double MinOverallScore { get; set; }
    public bool OnlyConfirmed { get; set; }
    public bool OnlyWithCompanyResponse { get; set; }
}

public class GetUserReviewsQueryValidator : AbstractValidator<GetUserReviewsQuery>
{
    public GetUserReviewsQueryValidator()
    {
        RuleFor(x => x.UserProfileId).GreaterThan(0)
            .WithMessage("CompanId mora biti veci od 0");

        RuleFor(x => x.FilterValue).MaximumLength(100)
            .WithMessage("FilterValue ne sme biti duzi od 100 karaktera");
    }
}

public class GetUserReviewsQueryHandler : IRequestHandler<GetUserReviewsQuery, IPaginatedList<FilteredReviewDto>>
{
    private readonly IDatabaseRepository<Review> _reviewsRepository;

    public GetUserReviewsQueryHandler(IDatabaseRepository<Review> reviewsRepository)
    {
        _reviewsRepository = reviewsRepository;
    }

    public async Task<IPaginatedList<FilteredReviewDto>> Handle(GetUserReviewsQuery request, CancellationToken cancellationToken)
    {
        var specification = new Specification<Review>(r => r.ReviewerId == request.UserProfileId);
        
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
        
        return await _reviewsRepository.GetAndProjectAsPaginatedList<FilteredReviewDto>(specification, request.QueryArgs);
    }
}