using AutoMapper;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Domain.Models;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Queries;

public class GetCompaniesQuery
{
    
}

public class CompanyDto : IMapFrom<Company>
{
    public string Name { get; set; }
    public bool IsVerified { get; set; }
    public string City { get; set; }
    public string? Location { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Company, CompanyDto>()
            .ForMember(dest => dest.City,
                options => options.MapFrom((src) => src.City.Name));
    }
}