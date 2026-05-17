using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Commands;

public class EditCompanyCommand : IRequest<Unit>
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
    public int CityId { get; set; }
    public ICollection<EditCompanyImageDto>? Images { get; set; }
}

public class EditCompanyCommandValidator : AbstractValidator<EditCompanyCommand>
{
    public EditCompanyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Naziv kompanije je obavezan")
            .MaximumLength(200).WithMessage("Naziv kompanije ne sme biti duzi od 200 karaktera");

        RuleFor(x => x.Description).NotEmpty().WithMessage("Opis kompanije je obavezan")
            .MaximumLength(1000).WithMessage("Opis kompanije ne sme biti duzi od 1000 karaktera");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Lokacija je obavezna")
            .MaximumLength(200).WithMessage("Lokacija ne sme biti duza od 200 karaktera")
            .When(x => !string.IsNullOrEmpty(x.Location));

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Ulica je obavezna")
            .MaximumLength(200).WithMessage("Ulica ne sme biti duza od 200 karaktera");

        RuleFor(x => x.HouseNumber)
            .NotEmpty().WithMessage("Kucni broj je obavezan")
            .MaximumLength(50).WithMessage("Kucni broj ne sme biti duzi od 50 karaktera");

        RuleFor(x => x.Verifier)
            .NotEmpty().WithMessage("Broj ili Email je obavezan")
            .MaximumLength(200).WithMessage("Broj ili Email ne smeju biti duzi od 200 karaktera");

        RuleFor(x => x.Latitude).NotEmpty().WithMessage("Koordinate su obavezne");

        RuleFor(x => x.Longitude).NotEmpty().WithMessage("Koordinate su obavezne");

        RuleFor(x => x.CompanyPib)
            .MaximumLength(20).WithMessage("PIB kompanije ne sme biti duzi od 20 karaktera")
            .When(x => !string.IsNullOrEmpty(x.CompanyPib));

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Kategorija je obavezna");

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("Grad je obavezan");

        RuleFor(x => x.Images)
            .Must(images => images.All(img => !string.IsNullOrEmpty(img.Path)))
            .WithMessage("Sve slike moraju imati putanju")
            .When(x => x.Images != null && x.Images.Any());
    }
}

public class EditCompanyCommandHandler : IRequestHandler<EditCompanyCommand, Unit>
{
    private readonly IDatabaseRepository<Company> _companyRepository;
    private readonly IDatabaseRepository<Category> _categoryRepository;
    private readonly IDatabaseRepository<City> _cityRepository;
    private readonly IDatabaseRepository<CompanyImage> _companyImageRepository;

    public EditCompanyCommandHandler(
        IDatabaseRepository<Company> companyRepository,
        IDatabaseRepository<Category> categoryRepository,
        IDatabaseRepository<City> cityRepository,
        IDatabaseRepository<CompanyImage> companyImageRepository)
    {
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
        _cityRepository = cityRepository;
        _companyImageRepository = companyImageRepository;
    }

    public async Task<Unit> Handle(EditCompanyCommand request, CancellationToken cancellationToken)
    {
        var oldCompany = await _companyRepository.GetSingleBySpec(new Specification<Company>(c => 
            c.Id != request.CompanyId &&
            ((c.Name.ToLower() == request.Name.ToLower() && c.CityId == request.CityId) || 
            (!string.IsNullOrEmpty(request.CompanyPib) && c.CompanyPib == request.CompanyPib))
            ));
        if(oldCompany is not null)
            throw new AppException("Kompanija koju pokusavate da kreirate vec postoji");
        
        var company = await _companyRepository.GetSingleBySpec(new Specification<Company>(c => c.Id == request.CompanyId)
            .AddInclude(c => c.Images));
        if(company is null)
            throw new AppException("Kompanija ne postoji");
        
        var category = await _categoryRepository.GetById(request.CategoryId);
        if (category is null)
            throw new AppException("Kategorija ne postoji");
        
        var city = await _cityRepository.GetById(request.CityId);
        if (city is null)
            throw new AppException("Grad ne postoji");

        
        company.Name = request.Name;
        company.Description = request.Description;
        company.Location = request.Location;
        company.Street = request.Street;
        company.HouseNumber = request.HouseNumber;
        company.Verifier = request.Verifier;
        company.IsEmailVerifier = request.IsEmailVerifier;
        company.PublicPageUrl = request.PublicPageUrl;
        company.Latitude = request.Latitude;
        company.Longitude = request.Longitude;
        company.CompanyPib = request.CompanyPib;
        company.CategoryId = request.CategoryId;
        company.CityId = request.CityId;

        //Handle CompanyImages
        await _companyImageRepository.DeleteRange(company.Images);
        if (request.Images is not null && request.Images.Any())
        {
            foreach (var image in request.Images)
            {
                company.Images.Add(new CompanyImage
                {
                    Title = image.Title,
                    Path = image.Path,
                    IsProfile = image.IsProfile,
                });
            }
        }

        await _companyRepository.Update(company);

        return Unit.Value;
    }
}