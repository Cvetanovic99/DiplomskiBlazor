using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Application.UseCases.Reviews.Queries;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.CompanyResposnes.Commands;

public class EditCompanyResponseCommand : IRequest<CompanyResponseDto>
{
    public int Id { get; set; }
    public string Text { get; set; }
    public IList<EditReviewImageDto> Images { get; set; } =  new List<EditReviewImageDto>();
}

public class EditCompanyResponseCommandValidator : AbstractValidator<EditCompanyResponseCommand>
{
    public EditCompanyResponseCommandValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
    }
}

public class EditCompanyResponseCommandHandler : IRequestHandler<EditCompanyResponseCommand, CompanyResponseDto>
{
    private readonly IDatabaseRepository<CompanyResponse>  _repository;
    private readonly IDatabaseRepository<CompanyResponseImage>  _companyResponseImageRepository;

    public EditCompanyResponseCommandHandler(IDatabaseRepository<CompanyResponse> repository,
        IDatabaseRepository<CompanyResponseImage> companyResponseImageRepository)
    {
        _repository = repository;
        _companyResponseImageRepository = companyResponseImageRepository;
    }
    public async Task<CompanyResponseDto> Handle(EditCompanyResponseCommand request, CancellationToken cancellationToken)
    {
        var companyResponse = await _repository.GetSingleBySpec(new Specification<CompanyResponse>(r => r.Id == request.Id)
            .AddInclude(r => r.Images));

        if (companyResponse == null)
            throw new AppException("Odgovor ne postoji");
        
        IList<CompanyResponseImage> images = request.Images.Select(i => new CompanyResponseImage{Title = i.Title, Path = i.Path}).ToList();
        
        companyResponse.Text = request.Text;
        
        await _companyResponseImageRepository.DeleteRange(companyResponse.Images);

        companyResponse.Images = images;
        
        await _repository.Update(companyResponse);

        return await _repository.GetSingleAndProject<CompanyResponseDto>(
            new Specification<CompanyResponse>(r => r.Id == companyResponse.Id));
    }
}