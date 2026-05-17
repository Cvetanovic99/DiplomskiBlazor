using Diplomski.RatingHub.Application.Enums;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Models;
using Diplomski.RatingHub.Application.Interfaces.Notifications;
using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.Models.Dtos;
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
                Name = createCompanyDto.Name.Trim(),
                Description =  createCompanyDto.Description,
                Location =  createCompanyDto.Location,
                Street =   createCompanyDto.Street,
                HouseNumber =   createCompanyDto.HouseNumber,
                Verifier =  createCompanyDto.Verifier.Trim(),
                IsEmailVerifier = createCompanyDto.IsEmailVerifier,
                PublicPageUrl = createCompanyDto.PublicPageUrl?.Trim(),
                Latitude =   createCompanyDto.Latitude,
                Longitude =    createCompanyDto.Longitude,
                CompanyPib =  createCompanyDto.CompanyPib?.Trim(),
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
        if (string.IsNullOrEmpty(createCompanyDto.Verifier))
            throw new AppException("Doslo je do greske");
        
        createCompanyDto.IsEmailVerifier = RegisterUserDto.IsEmail(createCompanyDto.Verifier);
        
        return await Send(
            new CreateCompanyCommand 
            {
                Name = createCompanyDto.Name.Trim(),
                Description =  createCompanyDto.Description,
                Location =  createCompanyDto.Location,
                Street =   createCompanyDto.Street,
                HouseNumber =   createCompanyDto.HouseNumber,
                Verifier =  createCompanyDto.Verifier.Trim(),
                IsEmailVerifier = createCompanyDto.IsEmailVerifier,
                PublicPageUrl = createCompanyDto.PublicPageUrl?.Trim(),
                Latitude =   createCompanyDto.Latitude,
                Longitude =    createCompanyDto.Longitude,
                CompanyPib =  createCompanyDto.CompanyPib?.Trim(),
                OwnerId = createCompanyDto.OwnerId,
                CategoryId = createCompanyDto.CategoryId,
                CityId = createCompanyDto.CityId,
                Images =  createCompanyDto.Images,
                ClaimCompanyIdentifier = null,//It's already created by owner
                AnonymousEditIdentifier = null//Owner will be able to edit
            });
    }

    public async Task EditCompany(EditCompanyDto editCompanyDto)
    {
        if (string.IsNullOrEmpty(editCompanyDto.Verifier))
            throw new AppException("Doslo je do greske");
        
        editCompanyDto.IsEmailVerifier = RegisterUserDto.IsEmail(editCompanyDto.Verifier);
        
        await Send(new EditCompanyCommand
        {
            CompanyId = editCompanyDto.CompanyId,
            Name = editCompanyDto.Name.Trim(),
            Description =  editCompanyDto.Description,
            Location =  editCompanyDto.Location,
            Street =   editCompanyDto.Street,
            HouseNumber =   editCompanyDto.HouseNumber,
            Verifier =  editCompanyDto.Verifier.Trim(),
            IsEmailVerifier = editCompanyDto.IsEmailVerifier,
            PublicPageUrl = editCompanyDto.PublicPageUrl?.Trim(),
            Latitude =   editCompanyDto.Latitude,
            Longitude =    editCompanyDto.Longitude,
            CompanyPib =  editCompanyDto.CompanyPib?.Trim(),
            CategoryId = editCompanyDto.CategoryId,
            CityId = editCompanyDto.CityId,
            Images =  editCompanyDto.Images,
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

    public async Task<CompanyDetailsDto> GetCompanyDetails(int companyId)
    {
        return await Send(new GetCompanyDetailsQuery { CompanyId = companyId });
    }

    public async Task<CompanyDetailsAdditionalDataDto> GetCompanyDetailsAdditionalData(int companyId)
    {
        return await  Send(new GetCompanyDetailsAdditionalDataQuery { CompanyId = companyId });
    }

    public async Task<bool> ValidateCompanyAnonymousEditIdentifier(int companyId, string companyAnonymousEditIdentifier)
    {
        await Task.Delay(500);
        return await Send(new ValidateCompanyAnonymousEditIdentifierQuery
        {
            CompanyId = companyId,
            AnonymousEditIdentifier = companyAnonymousEditIdentifier
        });
    }

    public async Task DeleteCompanyAsAnonymous(int companyId, bool isAdminDeleting = false)
    {
        await Send(new DeleteCompanyAsAnonymousCommand { CompanyId = companyId, IsAdminDeleting = isAdminDeleting});
    }

    public async Task DeleteCompanyAsOwner(int companyId)
    {
        await Send(new DeleteCompanyAsOwnerCommand { CompanyId = companyId });
    }

    public async Task<EditCompanyDto> GetCompanyForEdit(int companyId)
    {
        return await Send(new GetCompanyForEditQuery { CompanyId = companyId });
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