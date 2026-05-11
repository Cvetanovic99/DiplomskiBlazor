using AutoMapper;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Mapping;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Cities.Queries;

public class GetCityByIdQuery : IRequest<CityDto>
{
    public int CityId { get; set; }
}

public class GetCityByIdQueryValidator : AbstractValidator<GetCityByIdQuery>
{
    public GetCityByIdQueryValidator()
    {
        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("ID grada mora biti veći od 0");
    }
}

public class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery, CityDto>
{
    private readonly IDatabaseRepository<City> _cityRepository;
    private readonly IMapper _mapper;

    public GetCityByIdQueryHandler(IDatabaseRepository<City> cityRepository, IMapper mapper)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
    }

    public async Task<CityDto> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
    {
        var city = await _cityRepository.GetById(request.CityId);
        if (city == null)
            throw new ApplicationException("Grad ne postoji");

        return _mapper.Map<CityDto>(city);
    }
}
