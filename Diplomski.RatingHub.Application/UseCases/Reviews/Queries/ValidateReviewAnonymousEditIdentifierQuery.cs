using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Reviews.Queries;

public class ValidateReviewAnonymousEditIdentifierQuery : IRequest<bool>
{
    public int ReviewId { get; set; }
    public string AnonymousEditIdentifier { get; set; }
}

public class ValidateReviewAnonymousEditIdentifierQueryValidator : AbstractValidator<ValidateReviewAnonymousEditIdentifierQuery>
{
    public ValidateReviewAnonymousEditIdentifierQueryValidator()
    {
        RuleFor(model => model.ReviewId).GreaterThan(0).WithMessage("ReviewId je obavezan");
        RuleFor(model => model.AnonymousEditIdentifier).NotEmpty().WithMessage("Identifier je obavezan");
    }
}

public class ValidateReviewAnonymousEditIdentifierQueryHandler : IRequestHandler<ValidateReviewAnonymousEditIdentifierQuery, bool>
{
    private readonly IDatabaseRepository<Review> _reviewRepository;

    public ValidateReviewAnonymousEditIdentifierQueryHandler(IDatabaseRepository<Review> reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<bool> Handle(ValidateReviewAnonymousEditIdentifierQuery request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetCount(
            new Specification<Review>(r => r.Id == request.ReviewId && r.AnonymousEditIdentifier == request.AnonymousEditIdentifier));
        
        return review > 0;
    }
}