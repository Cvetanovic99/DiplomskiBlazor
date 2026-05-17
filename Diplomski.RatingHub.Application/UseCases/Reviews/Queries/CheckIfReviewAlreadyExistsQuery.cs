using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Reviews.Queries;

public class CheckIfReviewAlreadyExistsQuery : IRequest<bool>
{
    public string Key { get; set; }
    public int CompanyId  { get; set; }
}

public class CheckIfReviewAlreadyExistsQueryValidator : AbstractValidator<CheckIfReviewAlreadyExistsQuery>
{
    public CheckIfReviewAlreadyExistsQueryValidator()
    {
        RuleFor(x => x.Key).NotNull().NotEmpty().WithMessage("Kljuc je obavezan");
        RuleFor(x => x.CompanyId).NotNull().GreaterThan(0).WithMessage("CompanyId je obavezan");
    }
}

public class CheckIfReviewAlreadyExistsQueryHandler : IRequestHandler<CheckIfReviewAlreadyExistsQuery, bool>
{
    private readonly IDatabaseRepository<Review> _repository;

    public CheckIfReviewAlreadyExistsQueryHandler(IDatabaseRepository<Review> repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(CheckIfReviewAlreadyExistsQuery request, CancellationToken cancellationToken)
    {
        int reviewCount = await _repository.GetCount(new Specification<Review>(r => 
            r.CompanyId == request.CompanyId &&
            r.ReviewerIdentifier == request.Key));
        
        return reviewCount > 0;
    }
}