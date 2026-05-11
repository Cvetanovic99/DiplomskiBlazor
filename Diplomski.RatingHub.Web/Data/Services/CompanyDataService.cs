using Diplomski.RatingHub.Application.Enums;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Notifications;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.Companies.Commands;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using NanoidDotNet;

namespace Diplomski.RatingHub.Web.Data.Services;

public class CompanyDataService : DataServiceBase, ICompanyDataService
{
    private readonly IEmailNotificationService _emailNotificationService;
    private readonly ISmsNotificationService _smsNotificationService;
    public CompanyDataService(
        IServiceScopeFactory serviceScopeFactory, 
        IEmailNotificationService emailNotificationService,
        ISmsNotificationService smsNotificationService) : base(serviceScopeFactory)
    {
        _emailNotificationService = emailNotificationService;
        _smsNotificationService = smsNotificationService;
    }
    public async Task<IPaginatedList<CompanyDto>> GetCompanies(string filterValue, int cityId, QueryArgs queryArgs)
    {
        return await Send(new GetCompaniesQuery
        {
            FilterValue = filterValue,
            CityId = cityId,
            QueryArgs = queryArgs
        });
    }

    public async Task<CreateCompanyAsAnonymousResponse> CreateCompanyAsAnonymous(CreateCompanyDto createCompanyDto)
    {
        if (string.IsNullOrEmpty(createCompanyDto.Verifier))
            throw new AppException("Doslo je do greske");
        
        createCompanyDto.IsEmailVerifier = RegisterUserDto.IsEmail(createCompanyDto.Verifier);
        createCompanyDto.ClaimCompanyIdentifier = await Nanoid.GenerateAsync(Nanoid.Alphabets.LettersAndDigits, 15);
        createCompanyDto.AnonymousEditIdentifier = await Nanoid.GenerateAsync(Nanoid.Alphabets.LettersAndDigits, 15);

        await NotifyOwnerAboutCompanyCreation(createCompanyDto);
        
        
        int companyId = await Send(
            new CreateCompanyCommand 
            {
                Name = createCompanyDto.Name,
                Description =  createCompanyDto.Description,
                Location =  createCompanyDto.Location,
                Street =   createCompanyDto.Street,
                HouseNumber =   createCompanyDto.HouseNumber,
                Verifier =  createCompanyDto.Verifier,
                IsEmailVerifier = createCompanyDto.IsEmailVerifier,
                PublicPageUrl = createCompanyDto.PublicPageUrl,
                Latitude =   createCompanyDto.Latitude,
                Longitude =    createCompanyDto.Longitude,
                CompanyPib =  createCompanyDto.CompanyPib,
                OwnerId = null, //When user who creates company is Anonymous
                CategoryId = createCompanyDto.CategoryId,
                CityId = createCompanyDto.CityId,
                Images =  createCompanyDto.Images,
                ClaimCompanyIdentifier = createCompanyDto.ClaimCompanyIdentifier,
                AnonymousEditIdentifier = createCompanyDto.AnonymousEditIdentifier
            });

        return new CreateCompanyAsAnonymousResponse
        {
            CompanyId = companyId,
            AnonymousEditIdentifier = createCompanyDto.AnonymousEditIdentifier
        };
    }

    public async Task<int> CreateCompanyAsOwner(CreateCompanyDto createCompanyDto)
    {
        createCompanyDto.IsEmailVerifier = RegisterUserDto.IsEmail(createCompanyDto.Verifier);
        
        return await Send(
            new CreateCompanyCommand 
            {
                Name = createCompanyDto.Name,
                Description =  createCompanyDto.Description,
                Location =  createCompanyDto.Location,
                Street =   createCompanyDto.Street,
                HouseNumber =   createCompanyDto.HouseNumber,
                Verifier =  createCompanyDto.Verifier,
                IsEmailVerifier = createCompanyDto.IsEmailVerifier,
                PublicPageUrl = createCompanyDto.PublicPageUrl,
                Latitude =   createCompanyDto.Latitude,
                Longitude =    createCompanyDto.Longitude,
                CompanyPib =  createCompanyDto.CompanyPib,
                OwnerId = createCompanyDto.OwnerId,
                CategoryId = createCompanyDto.CategoryId,
                CityId = createCompanyDto.CityId,
                Images =  createCompanyDto.Images,
                ClaimCompanyIdentifier = null,//It's already created by owner
                AnonymousEditIdentifier = null//Owner will be able to edit
            });
    }

    public async Task<IPaginatedList<FilteredCompanyDto>> GetFilteredCompanies(int cityId, int categoryId, string filterValue, double overallRatingGrade,
        QueryArgs queryArgs, CompanyClaimStatusFilterOptions claimStatus,
        CompanyVerificationStatusFilterOptions verificationStatus)
    {
        return await Send(new GetFilteredCompaniesQuery
        {
            CityId = cityId,
            CategoryId = categoryId,
            FilterValue = filterValue,
            OverallRatingGrade = overallRatingGrade,
            QueryArgs = queryArgs,
            ClaimStatus = claimStatus,
            VerificationStatus = verificationStatus
        });
    }

    public async Task<IEnumerable<PopularCompanyDto>> GetPopularCompanies(int cityId, int categoryId, int take)
    {
        return await Send(new GetPopularCompaniesQuery
        {
            CityId = cityId,
            CategoryId = categoryId,
            Take = take
        });
    }

    private async Task NotifyOwnerAboutCompanyCreation(CreateCompanyDto createCompanyDto)
    {
        try
        {
            if (createCompanyDto.IsEmailVerifier)
                await _emailNotificationService.NotifyOwnerAboutCompanyCreationAsync(
                    createCompanyDto.Verifier, createCompanyDto.Name, createCompanyDto.ClaimCompanyIdentifier!);
            else
                await _smsNotificationService.NotifyOwnerAboutCompanyCreationWithEmail(
                    createCompanyDto.Name, createCompanyDto.ClaimCompanyIdentifier!);
        }
        catch (Exception e)
        {
            throw new AppException("Doslo je do greske prilikom slanja notifikacije vlasniku kompanije. Molimo pokusajte ponovo kasnije.");
        }
    }
}