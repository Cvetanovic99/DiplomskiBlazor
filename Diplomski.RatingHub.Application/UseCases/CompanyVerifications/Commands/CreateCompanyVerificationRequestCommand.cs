using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.CompanyVerifications.Commands;

public class CreateCompanyVerificationRequestCommand : IRequest<UserCompanyVerificationRequestDto>
{
    public required string ContactEmail { get; set; }
    public string? Description { get; set; }
    public required string Identifier { get; set; }
    public int OwnerId { get; set; }
    public int CompanyId { get; set; }
}

public class
    CreateCompanyVerificationRequestCommandValidator : AbstractValidator<CreateCompanyVerificationRequestCommand>
{
    public CreateCompanyVerificationRequestCommandValidator()
    {
        RuleFor(x => x.ContactEmail).NotNull().NotEmpty();
        RuleFor(x => x.OwnerId).GreaterThan(0);
        RuleFor(x => x.CompanyId).GreaterThan(0);
    }
}

public class
    CreateCompanyVerificationRequestCommandHandler : IRequestHandler<CreateCompanyVerificationRequestCommand, UserCompanyVerificationRequestDto>
{
    private readonly IDatabaseRepository<CompanyVerificationRequest> _repository;

    public CreateCompanyVerificationRequestCommandHandler(IDatabaseRepository<CompanyVerificationRequest> repository)
    {
        _repository = repository;
    }

    public async Task<UserCompanyVerificationRequestDto> Handle(CreateCompanyVerificationRequestCommand request, CancellationToken cancellationToken)
    {
        var oldVerificationRequest = await _repository.GetCount(new Specification<CompanyVerificationRequest>(r =>
            r.OwnerId == request.OwnerId &&
            r.CompanyId == request.CompanyId && 
            r.Status != CompanyVerificationRequestStatus.Dismissed));

        if (oldVerificationRequest > 0)
            throw new AppException("Morate sacekati da se obrati prethodni zahtev");
        
        var verificationRequest = new CompanyVerificationRequest
        {
            Status = CompanyVerificationRequestStatus.Pending,
            ContactEmail = request.ContactEmail,
            Description = request.Description,
            Identifier = request.Identifier,
            OwnerId = request.OwnerId,
            CompanyId = request.CompanyId
        };
        
        await _repository.Insert(verificationRequest);
        
        return new UserCompanyVerificationRequestDto
        {
            Id =verificationRequest.Id,
            Status = verificationRequest.Status,
            ContactEmail = verificationRequest.ContactEmail,
            Description = verificationRequest.Description,
            Identifier = verificationRequest.Identifier,
        };
    }
}