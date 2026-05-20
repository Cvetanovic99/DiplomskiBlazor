using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Commands;

public class SetCompanyOwnerCommand : IRequest<Unit>
{
    public int UserProfileId { get; set; }
    public required string ClaimCompanyIdentifier { get; set; }
}

public class SetCompanyOwnerCommandValidator : AbstractValidator<SetCompanyOwnerCommand>
{
    public SetCompanyOwnerCommandValidator()
    {
        RuleFor(x => x.UserProfileId).NotNull();
        RuleFor(x => x.ClaimCompanyIdentifier).NotEmpty().MaximumLength(15).MinimumLength(15);
    }
}

public class SetCompanyOwnerCommandHandler : IRequestHandler<SetCompanyOwnerCommand, Unit>
{
    private readonly IDatabaseRepository<Company> _companyRepository;

    public SetCompanyOwnerCommandHandler(IDatabaseRepository<Company> companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Unit> Handle(SetCompanyOwnerCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetSingleBySpec(new Specification<Company>(c => c.ClaimCompanyIdentifier == request.ClaimCompanyIdentifier));
        if (company is null)
            throw new AppException("Ova kompanija ne postoji");

        if (company.OwnerId != null)
            throw new AppException("Kompanija vec ima vlasnika");
        
        company.OwnerId = request.UserProfileId;
        
        await _companyRepository.Update(company);
        
        return Unit.Value;
    }
}