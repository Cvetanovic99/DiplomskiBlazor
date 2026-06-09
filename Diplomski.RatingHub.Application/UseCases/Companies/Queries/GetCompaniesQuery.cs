using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Queries;

public class GetCompaniesQuery : IRequest<IPaginatedList<CompanyDto>>
{
    public QueryArgs QueryArgs { get; set; }
    public string FilterValue { get; set; }
    public int CityId { get; set; }
}

public class GetCompaniesQueryValidator : AbstractValidator<GetCompaniesQuery>
{
    public GetCompaniesQueryValidator()
    {
        RuleFor(x => x.FilterValue).NotNull().WithMessage("Vrednost za filter ne sme biti prazna");
        RuleFor(x => x.CityId).GreaterThan(0).WithMessage("Morate izabrati adresu");
        RuleFor(x => x.QueryArgs).NotNull().WithMessage("QueryArgs ne mogu biti prazni");
    }
}

public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, IPaginatedList<CompanyDto>>
{
    private readonly IDatabaseRepository<Company> _companyRepository;

    public GetCompaniesQueryHandler(IDatabaseRepository<Company> companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<IPaginatedList<CompanyDto>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var spec = new Specification<Company>(c => c.CityId == request.CityId &&
            (c.Name.Contains(request.FilterValue) ||
             (c.CompanyPib != null && c.CompanyPib.Contains(request.FilterValue))))
            .ApplyOrderByDescending(x => x.IsSponsored)
            .ApplyThenOrderByDescending(x => x.OwnerId != null);
        

        return await _companyRepository.GetAndProjectAsPaginatedList<CompanyDto>(
            spec, request.QueryArgs);
    }
}

public class CompanyDto : IMapFrom<Company>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsVerified { get; set; }
    public string City { get; set; }
    public string? Location { get; set; }
    public string? ProfileImagePath { get; set; }
    public bool HasOwner  { get; set; }
    public string? CompanyPib  { get; set; }
    public bool IsSponsored { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Company, CompanyDto>()
            .ForMember(dest => dest.City,
                options => options.MapFrom((src) => src.City.Name))
            .ForMember(dest => dest.ProfileImagePath,
                options => options.MapFrom((src) => 
                    src.Images.FirstOrDefault(i => i.IsProfile).Path))
            .ForMember(dest => dest.HasOwner,
                options => options.MapFrom(src => src.OwnerId != null));
    }
}