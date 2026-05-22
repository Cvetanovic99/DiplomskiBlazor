using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.CompanyResposnes.Commands;

public class CreateCompanyResponseCommand : IRequest<CompanyResponseDto>
{
    public required string Text { get; set; }
    
    public int CompanyId { get; set; }
    public int ReviewId { get; set; }
    public IList<CreateReviewImageDto> Images { get; set; } =  new List<CreateReviewImageDto>();
}

public class CreateCompanyResponseCommandValidator : AbstractValidator<CreateCompanyResponseCommand>
{
    public CreateCompanyResponseCommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.CompanyId).GreaterThan(0);
        RuleFor(x => x.ReviewId).GreaterThan(0);
    }
}

public class CreateCompanyResponseCommandHandler : IRequestHandler<CreateCompanyResponseCommand, CompanyResponseDto>
{
    private readonly IDatabaseRepository<CompanyResponse>  _repository;

    public CreateCompanyResponseCommandHandler(IDatabaseRepository<CompanyResponse> repository)
    {
        _repository = repository;
    }

    public async Task<CompanyResponseDto> Handle(CreateCompanyResponseCommand request, CancellationToken cancellationToken)
    {
        var oldResponse = await _repository.GetSingleBySpec(new Specification<CompanyResponse>(r => r.ReviewId == request.ReviewId));
        if (oldResponse is not null)
            throw new AppException("Vec ste greirali odgovor za ovu ocenu");
        
        IList<CompanyResponseImage> images = request.Images.Select(i => new CompanyResponseImage{Title = i.Title, Path = i.Path}).ToList();

        var companyResponse = new CompanyResponse
        {
            Text = request.Text,
            CompanyId = request.CompanyId,
            ReviewId = request.ReviewId,
            Images = images
        };

        await _repository.Insert(companyResponse);
        
        return await _repository.GetSingleAndProject<CompanyResponseDto>(new Specification<CompanyResponse>(r => r.Id == companyResponse.Id));
    }
}

