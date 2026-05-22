using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Interfaces.Storage;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.CompanyResposnes.Commands;

public class DeleteCompanyResponseCommand : IRequest<Unit>
{
    public int CompanyResponseId  { get; set; }
}

public class DeleteCompanyResponseCommandValidator : AbstractValidator<DeleteCompanyResponseCommand>
{
    public DeleteCompanyResponseCommandValidator()
    {
        RuleFor(x => x.CompanyResponseId).NotNull().GreaterThan(0);
    }
}

public class DeleteCompanYResponseCommandHandler : IRequestHandler<DeleteCompanyResponseCommand, Unit>
{
    private readonly IDatabaseRepository<CompanyResponse> _companyResponseRepository;
    private readonly IDatabaseRepository<CompanyResponseImage>  _companyResponseImageRepository;
    private readonly IFileService _fileService;

    public DeleteCompanYResponseCommandHandler(IDatabaseRepository<CompanyResponse> companyResponseRepository,
        IDatabaseRepository<CompanyResponseImage> companyResponseImageRepository, 
        IFileService fileService)
    {
        _companyResponseRepository = companyResponseRepository;
        _companyResponseImageRepository = companyResponseImageRepository;
        _fileService = fileService;
    }

    public async Task<Unit> Handle(DeleteCompanyResponseCommand request, CancellationToken cancellationToken)
    {
        var companyResponse = await _companyResponseRepository.GetSingleBySpec(
            new Specification<CompanyResponse>(r => r.Id == request.CompanyResponseId)
                .AddInclude(r => r.Images));

        if (companyResponse is null)
            throw new AppException("Odgovor ne postoji");

        if (companyResponse.Images.Any())
        {
            foreach (var image in companyResponse.Images)
            {
                _fileService.DeleteImage(image.Path);
            }
            
            await _companyResponseImageRepository.DeleteRange(companyResponse.Images);
        }

        await _companyResponseRepository.Delete(companyResponse);
        
        return Unit.Value;
    }
}