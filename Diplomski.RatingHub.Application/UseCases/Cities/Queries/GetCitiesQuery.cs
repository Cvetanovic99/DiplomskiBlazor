using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Cities.Queries;

public class GetCitiesQuery : IRequest<IPaginatedList<CityDto>>
{
    public required string FilterValue { get; set; }
    public required QueryArgs QueryArgs { get; set; }
}

public class GetCitiesQueryValidator : AbstractValidator<GetCitiesQuery>
{
    public GetCitiesQueryValidator()
    {
        RuleFor(q => q.FilterValue).NotNull();
        RuleFor(q => q.QueryArgs).NotNull();
    }
}

public class GetCitiesQueryHandler : IRequestHandler<GetCitiesQuery, IPaginatedList<CityDto>>
{
    private readonly IDatabaseRepository<City> _cityRepository;

    public GetCitiesQueryHandler(IDatabaseRepository<City> cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<IPaginatedList<CityDto>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
    {
        var citySpecification = new Specification<City>(
            c => c.Name.Contains(request.FilterValue));
        
        return await _cityRepository.GetAndProjectAsPaginatedList<CityDto>(citySpecification, request.QueryArgs);
    }
}

public class CityDto : IMapFrom<City>
{
    public required string Name { get; set; } 
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}