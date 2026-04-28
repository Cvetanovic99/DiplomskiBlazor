using MediatR;

namespace Diplomski.RatingHub.Web.Data.Services;

public abstract class DataServiceBase(IServiceScopeFactory serviceScopeFactory)
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    
    protected async Task<T> Send<T>(IRequest<T> request) 
    {
        using (var serviceScope = _serviceScopeFactory.CreateScope())
        {
            var scopedMediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();
            return await scopedMediator.Send(request);
        }
    } 
}