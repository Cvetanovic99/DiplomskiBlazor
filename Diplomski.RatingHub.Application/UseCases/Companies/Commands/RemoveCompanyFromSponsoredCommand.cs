using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Commands;

public class RemoveCompanyFromSponsoredCommand : IRequest<Unit>
{
    public int CompanyId { get; set; }
}

public class RemoveCompanyFromSponsoredCommandValidator : AbstractValidator<RemoveCompanyFromSponsoredCommand>
{
    public RemoveCompanyFromSponsoredCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
    }
}

public class RemoveCompanyFromSponsoredCommandHandler : IRequestHandler<RemoveCompanyFromSponsoredCommand, Unit>
{
    private readonly IDatabaseRepository<Company> _companyRepository;

    public RemoveCompanyFromSponsoredCommandHandler(IDatabaseRepository<Company> companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Unit> Handle(RemoveCompanyFromSponsoredCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetById(request.CompanyId);
        if (company is null)
            throw new AppException("Kompanija ne postoji");
        
        company.IsSponsored = false;
        company.SponsoredUntil = null;
        
        await _companyRepository.Update(company);
        
        return Unit.Value;
    }
}