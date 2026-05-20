using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Reviews.Queries;

public class GetReviewForAdminQuery : IRequest<FilteredReviewDto>
{
    public int ReviewId { get; set; }
}

public class GetReviewForAdminQueryValidator : AbstractValidator<GetReviewForAdminQuery>
{
    public GetReviewForAdminQueryValidator()
    {
        RuleFor(x => x.ReviewId).GreaterThan(0);
    }
}

public class GetReviewForAdminQueryHandler : IRequestHandler<GetReviewForAdminQuery, FilteredReviewDto>
{
    private readonly IDatabaseRepository<Review> _reviewsRepository;

    public GetReviewForAdminQueryHandler(IDatabaseRepository<Review> reviewsRepository)
    {
        _reviewsRepository = reviewsRepository;
    }

    public async Task<FilteredReviewDto> Handle(GetReviewForAdminQuery request, CancellationToken cancellationToken)
    {
        var review = await _reviewsRepository.GetSingleAndProject<FilteredReviewDto>(new Specification<Review>(r => r.Id == request.ReviewId));
        if (review is null)
            throw new AppException("Ocena ili odgovor kompanije ne postoji");
        
        return review;
    }
}