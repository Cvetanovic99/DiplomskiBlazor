using AutoMapper;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Application.UseCases.UserProfiles.Queries;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.UserProfiles.Commands;

public class EditUserProfileCommand : IRequest<UserProfileDto>
{
    public int UserProfileId { get; set; }
    public string Name { get; set; } 
    public string Surname { get; set; }
    public string? ProfileImagePath { get; set; }
}

public class EditUserProfileCommandValidator : AbstractValidator<EditUserProfileCommand>
{
    public EditUserProfileCommandValidator()
    {
        RuleFor(x => x.UserProfileId).GreaterThan(0);
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.Surname).NotEmpty();
    }
}

public class EditUserProfileCommandHandler : IRequestHandler<EditUserProfileCommand, UserProfileDto>
{
    private readonly IDatabaseRepository<UserProfile> _repository;
    private readonly IDatabaseRepository<UserImage> _userImagerepository;
    private readonly IMapper _mapper;

    public EditUserProfileCommandHandler(IDatabaseRepository<UserProfile> repository, 
        IDatabaseRepository<UserImage> userImagerepository, 
        IMapper mapper)
    {
        _repository = repository;
        _userImagerepository = userImagerepository;
        _mapper = mapper;
    }

    public async Task<UserProfileDto> Handle(EditUserProfileCommand request, CancellationToken cancellationToken)
    {
       var userProfile = await _repository.GetSingleBySpec(new Specification<UserProfile>(u => u.Id == request.UserProfileId)
           .AddInclude(u => u.ProfileImage));

       if (userProfile == null)
           throw new AppException("Korisnik ne postoji");

       if (!string.IsNullOrEmpty(request.ProfileImagePath))
       {
           if (userProfile.ProfileImage != null)
           {
               // UPDATE postojece slike
               userProfile.ProfileImage.Path = request.ProfileImagePath;
               userProfile.ProfileImage.Title = "Bez naslova";
           }
           else
           {
               // DODAJ novu sliku
               userProfile.ProfileImage = new UserImage
               {
                   Title = "Bez naslova",
                   Path = request.ProfileImagePath,
                   UserId = userProfile.Id 
               };
           }
       }
       else
       {
           // BRISANJE slike
           if (userProfile.ProfileImage != null)
           {
               await _userImagerepository.Delete(userProfile.ProfileImage);
               userProfile.ProfileImage = null;
           }
       }
       
       userProfile.Name = request.Name;
       userProfile.Surname = request.Surname;
       
       await _repository.Update(userProfile);
       
       return _mapper.Map<UserProfileDto>(userProfile);
    }
}
