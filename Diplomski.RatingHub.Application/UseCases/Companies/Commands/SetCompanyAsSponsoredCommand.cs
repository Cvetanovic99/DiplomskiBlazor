using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Commands;

public class SetCompanyAsSponsoredCommand : IRequest<Unit>
{
    public int CompanyId { get; set; }
}

public class SetCompanyAsSponsoredCommandValidator : AbstractValidator<SetCompanyAsSponsoredCommand>
{
    public SetCompanyAsSponsoredCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
    }
}

public class SetCompanyAsSponsoredCommandHandler : IRequestHandler<SetCompanyAsSponsoredCommand, Unit>
{
    private readonly IDatabaseRepository<Company> _companyRepository;

    public SetCompanyAsSponsoredCommandHandler(IDatabaseRepository<Company> companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Unit> Handle(SetCompanyAsSponsoredCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetById(request.CompanyId);
        if (company is null)
            throw new AppException("Kompanija ne postoji");
        
        if(company.IsSponsored)
            throw new AppException("Kompanija je vec sponzorisana");
        
        company.IsSponsored = true;
        company.SponsoredUntil = DateTime.UtcNow.AddMonths(1);
        
        await _companyRepository.Update(company);
        
        return Unit.Value;
    }
}