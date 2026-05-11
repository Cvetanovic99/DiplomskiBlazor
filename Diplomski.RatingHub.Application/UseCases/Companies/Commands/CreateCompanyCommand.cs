using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using Diplomski.RatingHub.Web.Models;
using FluentValidation;
using MediatR;
using NanoidDotNet;

namespace Diplomski.RatingHub.Application.UseCases.Companies.Commands;

public class CreateCompanyCommand : IRequest<int>
{
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
    public int? OwnerId { get; set; }
    public int CategoryId { get; set; }
    public int CityId { get; set; }
    public string? ClaimCompanyIdentifier { get; set; }
    public string? AnonymousEditIdentifier { get; set; }
    public ICollection<CreateImageDto>? Images { get; set; }
}

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
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

        RuleFor(x => x.OwnerId)
            .GreaterThan(0).When(x => x.OwnerId.HasValue).WithMessage("ID vlasnika mora biti veci od 0");

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

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, int>
{
    private readonly IDatabaseRepository<Company> _companyRepository;
    private readonly IDatabaseRepository<Category> _categoryRepository;
    private readonly IDatabaseRepository<City> _cityRepository;
    private readonly IDatabaseRepository<UserProfile> _userRepository;

    public CreateCompanyCommandHandler(
        IDatabaseRepository<Company> companyRepository,
        IDatabaseRepository<Category> categoryRepository,
        IDatabaseRepository<City> cityRepository,
        IDatabaseRepository<UserProfile> userRepository)
    {
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
        _cityRepository = cityRepository;
        _userRepository = userRepository;
    }

    public async Task<int> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetSingleBySpec(
            new Specification<Category>(c=> c.Id == request.CategoryId)
                .AddInclude(c => c.RatingCriteria));
        if (category is null)
            throw new ApplicationException("Kategorija ne postoji");
        
        var city = await _cityRepository.GetById(request.CityId);
        if (city is null)
            throw new ApplicationException("Grad ne postoji");
        
        if (request.OwnerId.HasValue)
        {
            var owner = await _userRepository.GetById(request.OwnerId.Value);
            if (owner is null)
                throw new ApplicationException("Vlasnik kompanije ne postoji");
        }

        string? anonymousEditIdentifier = request.OwnerId.HasValue ? null : 
            await Nanoid.GenerateAsync(Nanoid.Alphabets.LettersAndDigits, 15);
        
        var company = new Company
        {
            Name = request.Name,
            Description = request.Description,
            Location = request.Location,
            Street = request.Street,
            HouseNumber = request.HouseNumber,
            Verifier = request.Verifier,
            IsEmailVerifier = request.IsEmailVerifier,
            PublicPageUrl = request.PublicPageUrl,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            CompanyPib = request.CompanyPib,
            OwnerId = request.OwnerId,
            CategoryId = request.CategoryId,
            CityId = request.CityId,
            IsVerified = false, // New companies start as unverified
            ReviewsCount = 0,
            IsAnonymousCreator = !request.OwnerId.HasValue,
            ClaimCompanyIdentifier = request.ClaimCompanyIdentifier,
            AnonymousEditIdentifier = request.AnonymousEditIdentifier
        };

        AddImagesToCompany(company, request.Images);
        AddRatingAggregatesToCompany(company, category);

        await _companyRepository.Insert(company);

        return company.Id;
    }

    private void AddImagesToCompany(Company company, ICollection<CreateImageDto>? images)
    {
        if (images is not null && images.Any())
        {
            foreach (var image in images)
            {
                company.Images.Add(new CompanyImage
                {
                    Title = image.Title,
                    Path = image.Path,
                    IsProfile = image.IsProfile,
                });
            }
        }
    }

    private void AddRatingAggregatesToCompany(Company company, Category category)
    {
        if (category.RatingCriteria.Any())
        {
            foreach (var ratingCriterion in category.RatingCriteria)
            {
                company.CompanyRatingAggregates.Add(new CompanyRatingAggregate
                {
                    RatingCriterionId = ratingCriterion.Id
                });
            }
        }
    }
}