using Diplomski.RatingHub.Application.Interfaces.Repositories;
using Diplomski.RatingHub.Application.Specifications;
using Diplomski.RatingHub.Domain.Models;
using FluentValidation;
using MediatR;

namespace Diplomski.RatingHub.Application.UseCases.Reviews.Commands;

public class LikeOrDislikeReviewCommand : IRequest<Unit>
{
    public int ReviewId { get; set; }
    public int UserId { get; set; }
}

public class LikeOrDislikeReviewCommandValidator : AbstractValidator<LikeOrDislikeReviewCommand>
{
    public LikeOrDislikeReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId).NotNull();
        RuleFor(x => x.UserId).NotNull();
    }
}

public class LikeOrDislikeReviewCommandHandler : IRequestHandler<LikeOrDislikeReviewCommand, Unit>
{
    private readonly IDatabaseRepository<Like> _likesRepository;

    public LikeOrDislikeReviewCommandHandler(IDatabaseRepository<Like> likesRepository)
    {
        _likesRepository = likesRepository;
    }

    public async Task<Unit> Handle(LikeOrDislikeReviewCommand request, CancellationToken cancellationToken)
    {
        var like = await _likesRepository.GetSingleBySpec(
            new Specification<Like>(l => l.ReviewId == request.ReviewId && l.UserId == request.UserId));
        
        if (like == null)
        {
            var newLike = new Like
            {
                ReviewId = request.ReviewId,
                UserId = request.UserId
            };
            
            await _likesRepository.Insert(newLike);
        }
        else
        {
            await _likesRepository.Delete(like);
        }
        
        return Unit.Value;
    }
}