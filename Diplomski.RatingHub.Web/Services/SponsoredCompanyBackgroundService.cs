using Diplomski.RatingHub.Application.UseCases.Companies.Commands;
using MediatR;

namespace Diplomski.RatingHub.Web.Services;

public class SponsoredCompanyBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public SponsoredCompanyBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = GetDelayUntilMidnight();
            await Task.Delay(delay, stoppingToken);//Executes every day in midnight

            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Send(new ProcessSponsoredCompaniesCommand(), stoppingToken);
        }
    }
    
    private TimeSpan GetDelayUntilMidnight()
    {
        var now = DateTime.Now;
        var nextMidnight = now.Date.AddDays(1);
        return nextMidnight - now;
    }
}