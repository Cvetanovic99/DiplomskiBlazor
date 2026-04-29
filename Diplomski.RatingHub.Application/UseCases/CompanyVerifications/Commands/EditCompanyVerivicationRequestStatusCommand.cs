using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.CompanyVerifications.Commands;

public class EditCompanyVerificationRequestStatusCommand : IRequest<Unit>
{
    public int RequestId { get; set; }
    public CompanyVerificationRequestStatus Status { get; set; }
}

public class EditCompanyVerificationRequestStatusCommandValidator : AbstractValidator<EditCompanyVerificationRequestStatusCommand>
{
    public EditCompanyVerificationRequestStatusCommandValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class EditCompanyVerificationRequestStatusCommandHandler : IRequestHandler<EditCompanyVerificationRequestStatusCommand, Unit>
{
    private readonly IDatabaseRepository<CompanyVerificationRequest> _repository;

    public EditCompanyVerificationRequestStatusCommandHandler(
        IDatabaseRepository<CompanyVerificationRequest> repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(EditCompanyVerificationRequestStatusCommand request, CancellationToken cancellationToken)
    {
        var verificationRequest = await _repository.GetSingleBySpec(
            new Specification<CompanyVerificationRequest>(x => x.Id == request.RequestId));

        if (verificationRequest is null)
            throw new ApplicationException("Zahtev za verifikaciju kompanije ne postoji");

        verificationRequest.Status = request.Status;

        await _repository.Update(verificationRequest);

        return Unit.Value;
    }
}