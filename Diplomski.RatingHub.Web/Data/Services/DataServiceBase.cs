using MediatR;

namespace Diplomski.RatingHub.Web.Data.Services;

public abstract class DataServiceBase(IMediator mediator)
{
    private readonly IMediator _mediator = mediator;

    protected async Task<T> Send<T>(IRequest<T> request) 
    {
        return await _mediator.Send(request);
    } 
}