using Diplomski.RatingHub.Application.UseCases.Cities.Queries;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyPages;

public partial class CompanyMapCard : IAsyncDisposable
{
    [Parameter] public CityDto City { get; set; }
    [Parameter] public IEnumerable<Companies.MapCompanyDto> Companies { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("dialogMap.init", City.Latitude, City.Longitude, 13);
            await JSRuntime.InvokeVoidAsync("dialogMap.setCompanies", Companies);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await JSRuntime.InvokeVoidAsync("dialogMap.destroy");
    }
}