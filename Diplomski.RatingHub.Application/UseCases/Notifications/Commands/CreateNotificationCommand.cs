using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Notifications.Commands;

public class CreateNotificationCommand : IRequest<Unit>
{
    public required string Title { get; set; }
    public required string Message { get; set; }
    public int RecipientId { get; set; }
    public int? ActorId { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
}

public class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Naslov za obavestenje je obavezan")
            .MaximumLength(200).WithMessage("Naslov ne sme biti duzi od 200 karaktera");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Poruka obavestenja je obavezna")
            .MaximumLength(1000).WithMessage("Poruka ne sme biti duza od 1000 karaktera");

        RuleFor(x => x.RecipientId)
            .GreaterThan(0).WithMessage("ID primaoca obavesti je obavezan");

        RuleFor(x => x.ActorId)
            .GreaterThan(0).When(x => x.ActorId.HasValue).WithMessage("ID aktera mora biti veci od 0");

        RuleFor(x => x.EntityType)
            .MaximumLength(100).WithMessage("Tip entiteta ne sme biti duzi od 100 karaktera")
            .When(x => !string.IsNullOrEmpty(x.EntityType));

        RuleFor(x => x.EntityId)
            .GreaterThan(0).When(x => x.EntityId.HasValue).WithMessage("ID entiteta mora biti veci od 0");
    }
}

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Unit>
{
    private readonly IDatabaseRepository<Notification> _repository;
    private readonly IDatabaseRepository<UserProfile> _userRepository;

    public CreateNotificationCommandHandler(
        IDatabaseRepository<Notification> repository,
        IDatabaseRepository<UserProfile> userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<Unit> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var recipient = await _userRepository.GetById(request.RecipientId);
        if (recipient is null)
            throw new ApplicationException("Primaoc obavestenja ne postoji");
        
        if (request.ActorId.HasValue)
        {
            var actor = await _userRepository.GetById(request.ActorId.Value);
            if (actor is null)
                throw new ApplicationException("Akter obavestenja ne postoji");
        }

        var notification = new Notification
        {
            Title = request.Title,
            Message = request.Message,
            RecipientId = request.RecipientId,
            ActorId = request.ActorId,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            IsRead = false
        };

        await _repository.Insert(notification);

        return Unit.Value;
    }
}