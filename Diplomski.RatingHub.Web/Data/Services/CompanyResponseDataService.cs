using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.UseCases.CompanyResposnes.Commands;
using Diplomski.RatingHub.Application.UseCases.Notifications.Commands;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Domain.Models;
using Diplomski.RatingHub.Web.Data.Interfaces;

namespace Diplomski.RatingHub.Web.Data.Services;

public class CompanyResponseDataService(IServiceScopeFactory serviceScopeFactory) : DataServiceBase(serviceScopeFactory), ICompanyResponseDataService
{
    public async Task<CompanyResponseDto> EditCompanyResponse(EditCompanyResponseDto editCompanyResponseDto)
    {
        return await Send(new EditCompanyResponseCommand
        {
            Id = editCompanyResponseDto.Id,
            Text = editCompanyResponseDto.Text,
            Images = editCompanyResponseDto.Images
        });
    }

    public async Task<CompanyResponseDto> CreateCompanyResponse(CreateCompanyResponseDto createCompanyResponseDto)
    {
        if (createCompanyResponseDto.ReviewOwnerId.HasValue && createCompanyResponseDto.ReviewOwnerId != 0)
        {

            await Send(new CreateNotificationCommand
            {
                Title = "Odgovor na ocenu",
                Message = $"Vlasnik kompanije: {createCompanyResponseDto.CompanyName}, je odgovorio na vase ocenjivanje",
                RecipientId = createCompanyResponseDto.ReviewOwnerId.Value,
                EntityType = nameof(CompanyResponse)
            });
        }

        return await Send(new CreateCompanyResponseCommand
        {
            Text = createCompanyResponseDto.Text,
            CompanyId = createCompanyResponseDto.CompanyId,
            ReviewId = createCompanyResponseDto.ReviewId,
            Images = createCompanyResponseDto.Images
        });
    }

    public async Task DeleteCompanyResponse(int companyResponseId)
    {
        await Send(new DeleteCompanyResponseCommand { CompanyResponseId = companyResponseId });
    }
}