using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.CompanyVerifications.Commands;

public class DeleteCompanyVerificationRequestCommand : IRequest<Unit>
{
    public int RequestId { get; set; }
}

public class DeleteCompanyVerificationRequestCommandValidator : AbstractValidator<DeleteCompanyVerificationRequestCommand>
{
    public DeleteCompanyVerificationRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0);
    }
}

public class DeleteCompanyVerificationRequestCommandHandler : IRequestHandler<DeleteCompanyVerificationRequestCommand, Unit>
{
    private readonly IDatabaseRepository<CompanyVerificationRequest> _repository;

    public DeleteCompanyVerificationRequestCommandHandler(
        IDatabaseRepository<CompanyVerificationRequest> repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteCompanyVerificationRequestCommand request, CancellationToken cancellationToken)
    {
        var verificationRequest = await _repository.GetSingleBySpec(
            new Specification<CompanyVerificationRequest>(x => x.Id == request.RequestId));

        if (verificationRequest is null)
            throw new ApplicationException("Zahtev za verifikaciju kompanije ne postoji");

        await _repository.Delete(verificationRequest);

        return Unit.Value;
    }
}
