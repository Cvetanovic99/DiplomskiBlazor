using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Reviews.Queries;

public class GetUserCompanyReviewsQuery : IRequest<IPaginatedList<FilteredReviewDto>>
{
    public int CompanId { get; set; }
    public string FilterValue { get; set; }
    public QueryArgs QueryArgs { get; set; } //For orderBy and pagination
    public double MinOverallScore { get; set; }
    public bool OnlyConfirmed { get; set; }
    public bool OnlyWithoutCompanyResponse { get; set; }
}

public class GetUserCompanyReviewsQueryValidator : AbstractValidator<GetUserCompanyReviewsQuery>
{
    public GetUserCompanyReviewsQueryValidator()
    {
        RuleFor(x => x.CompanId).GreaterThan(0)
            .WithMessage("CompanId mora biti veci od 0");

        RuleFor(x => x.FilterValue).MaximumLength(100)
            .WithMessage("FilterValue ne sme biti duzi od 100 karaktera");
    }
}

public class GetUserCompanyReviewsQueryHandler : IRequestHandler<GetUserCompanyReviewsQuery, IPaginatedList<FilteredReviewDto>>
{
    private readonly IDatabaseRepository<Review> _reviewsRepository;

    public GetUserCompanyReviewsQueryHandler(IDatabaseRepository<Review> reviewsRepository)
    {
        _reviewsRepository = reviewsRepository;
    }

    public async Task<IPaginatedList<FilteredReviewDto>> Handle(GetUserCompanyReviewsQuery request, CancellationToken cancellationToken)
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
        
        if (request.OnlyWithoutCompanyResponse)
        {
            specification.And(r => r.CompanyResponseId == null);
        }
        
        return await _reviewsRepository.GetAndProjectAsPaginatedList<FilteredReviewDto>(specification, request.QueryArgs);
    }
}