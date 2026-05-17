using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Queries;

public class ValidateCompanyAnonymousEditIdentifierQuery : IRequest<bool>
{
    public int CompanyId { get; set; }
    public string AnonymousEditIdentifier { get; set; }
}

public class ValidateCompanyAnonymousEditIdentifierQueryValidator : AbstractValidator<ValidateCompanyAnonymousEditIdentifierQuery>
{
    public ValidateCompanyAnonymousEditIdentifierQueryValidator()
    {
        RuleFor(model => model.CompanyId).GreaterThan(0).WithMessage("CompanyId je obavezan");
        RuleFor(model => model.AnonymousEditIdentifier).NotEmpty().WithMessage("Identifier je obavezan");
    }
}

public class ValidateCompanyAnonymousEditIdentifierQueryHandler : IRequestHandler<ValidateCompanyAnonymousEditIdentifierQuery, bool>
{
    private readonly IDatabaseRepository<Company> _companyRepository;

    public ValidateCompanyAnonymousEditIdentifierQueryHandler(IDatabaseRepository<Company> companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<bool> Handle(ValidateCompanyAnonymousEditIdentifierQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetCount(
            new Specification<Company>(c => c.Id == request.CompanyId && 
                                            c.AnonymousEditIdentifier == request.AnonymousEditIdentifier &&
                                            c.OwnerId == null));
        
        return company > 0;
    }
}