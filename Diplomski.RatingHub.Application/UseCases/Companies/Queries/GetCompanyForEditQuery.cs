using AutoMapper;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Application.UseCases.Cities.Queries;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Queries;

public class GetCompanyForEditQuery : IRequest<EditCompanyDto>
{
    public int CompanyId { get; set; }
}

public class GetCompanyForEditQueryValidator : AbstractValidator<GetCompanyDetailsQuery>
{
    public GetCompanyForEditQueryValidator()
    {
        RuleFor(x => x.CompanyId).GreaterThan(0).WithMessage("CompanyId mora biti veci od 0");
    }
}

public class GetCompanyForEditQueryHandler : IRequestHandler<GetCompanyForEditQuery, EditCompanyDto>
{
    private readonly IDatabaseRepository<Company> _companyRepository;

    public GetCompanyForEditQueryHandler(IDatabaseRepository<Company> companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<EditCompanyDto> Handle(GetCompanyForEditQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetSingleAndProject<EditCompanyDto>(
            new Specification<Company>(c => c.Id == request.CompanyId));
        if (company is null)
            throw new AppException("Trazena kompanija ne postoji");
        
        return company;
    }
}

public class EditCompanyDto : IMapFrom<Company>
{
    public int CompanyId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string Verifier { get; set; }
    public bool IsEmailVerifier { get; set; }
    public string? PublicPageUrl  { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? CompanyPib { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int CityId { get; set; }
    public CityDto City { get; set; }
    public int? OwnerId { get; set; }
    
    public ICollection<EditCompanyImageDto> Images { get; set; } = new List<EditCompanyImageDto>();
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Company, EditCompanyDto>()
            .ForMember(dest => dest.CompanyId, 
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CategoryName, 
                opt => opt.MapFrom(src => src.Category.Name));
    }
}

public class EditCompanyImageDto : IMapFrom<CompanyImage>
{
    public int SortOrder { get; set; }
    public bool IsProfile { get; set; }
    public string Title { get; set; }
    public string Path { get; set; }
}