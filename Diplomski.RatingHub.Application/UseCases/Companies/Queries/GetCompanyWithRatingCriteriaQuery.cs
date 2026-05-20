using AutoMapper;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Queries;

public class GetCompanyAndRatingCriteriaQuery : IRequest<CompanyWithRatingCriteriaDto>
{
    public int CompanyId { get; set; }
}

public class GetCompanyAndRatingCriteriaQueryValidator : AbstractValidator<GetCompanyAndRatingCriteriaQuery>
{
    public GetCompanyAndRatingCriteriaQueryValidator()
    {
        RuleFor(x => x.CompanyId).GreaterThan(0).WithMessage("CompanyId mora biti veci od 0");
    }
}

public class GetCompanyRatingCriteriaQueryHandler :  IRequestHandler<GetCompanyAndRatingCriteriaQuery, CompanyWithRatingCriteriaDto>
{
    private readonly IDatabaseRepository<Company> _companyRepository;
    private readonly IDatabaseRepository<RatingCriterion>  _ratingCriterionRepository;

    public GetCompanyRatingCriteriaQueryHandler(
        IDatabaseRepository<Company> companyRepository, 
        IDatabaseRepository<RatingCriterion> ratingCriterionRepository)
    {
        _companyRepository = companyRepository;
        _ratingCriterionRepository = ratingCriterionRepository;
    }

    public async Task<CompanyWithRatingCriteriaDto> Handle(GetCompanyAndRatingCriteriaQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetSingleAndProject<CompanyWithRatingCriteriaDto>(
            new Specification<Company>(c => c.Id == request.CompanyId));
        if (company == null)
            throw new AppException("Kompanija ne postoji");

        var criteria = await _ratingCriterionRepository.GetAndProject<CompanyRatingCriterionDto>(
            new Specification<RatingCriterion>(rc => rc.CategoryId == company.CategoryId && rc.IsActive)
                .ApplyOrderBy(rc => rc.SortOrder));

        company.RatingCriteria = criteria;

        return company;
    }
}

public class CompanyWithRatingCriteriaDto : IMapFrom<Company>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? ProfileImagePath { get; set; }
    public int? OwnerId { get; set; }
    public int CategoryId { get; set; }
    public ICollection<CompanyRatingCriterionDto> RatingCriteria { get; set; } = new List<CompanyRatingCriterionDto>();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Company, CompanyWithRatingCriteriaDto>()
            .ForMember(dest => dest.ProfileImagePath,
                opt => opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsProfile).Path));
    }
}

public class CompanyRatingCriterionDto : IMapFrom<RatingCriterion>
{
    public int Id { get; set; }
    public string Name { get; set; } 
    public int SortOrder { get; set; } 
}