using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Commands;

public class VerifyCompanyCommand : IRequest<Unit>
{
    public int CompanyId { get; set; }
}

public class VerifyCompanyCommandValidator : AbstractValidator<VerifyCompanyCommand>
{
    public VerifyCompanyCommandValidator()
    {
        RuleFor(x => x.CompanyId).GreaterThan(0);
    }
}

public class VerifyCompanyCommandHandler : IRequestHandler<VerifyCompanyCommand, Unit>
{
    private readonly IDatabaseRepository<Company> _repository;

    public VerifyCompanyCommandHandler(
        IDatabaseRepository<Company> repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(VerifyCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _repository.GetById(request.CompanyId);

        if (company is null)
            throw new ApplicationException("Kompanija ne postoji");

        company.IsVerified = true;

        await _repository.Update(company);

        return Unit.Value;
    }
}