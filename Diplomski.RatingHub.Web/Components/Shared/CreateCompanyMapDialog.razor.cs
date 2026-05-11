using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Diplomski.RatingHub.Web.Components.Shared;

public partial class CreateCompanyMapDialog : IDisposable
{
    [Parameter] public MapDataDto CityLocation { get; set; } = default!;
    [Parameter] public MapDataDto CompanyLocation { get; set; } = default!;
    private DotNetObjectReference<CreateCompanyMapDialog> _dotnetRef;

    private string _mapId = $"map-{Guid.NewGuid()}";

    private MapDataDto? _selectedLocation;
    private bool _hasSelection => _selectedLocation != null;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        _dotnetRef = DotNetObjectReference.Create(this);
        if (firstRender)
        {
            var initialLat = CompanyLocation.Latitude == 0.0 ? CityLocation.Latitude : CompanyLocation.Latitude;
            var initialLng = CompanyLocation.Longitude == 0.0 ? CityLocation.Longitude :  CompanyLocation.Longitude;

            await JSRuntime.InvokeVoidAsync("mapHelper.initMapWithMarker",
                _mapId,
                CityLocation.Latitude,
                CityLocation.Longitude,
                initialLat,
                initialLng,
                _dotnetRef);
        }
    }

    [JSInvokable]
    public Task OnMapClick(double lat, double lng)
    {
        _selectedLocation = new MapDataDto
        {
            Latitude = lat,
            Longitude = lng
        };

        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task Close()
    {
        await JSRuntime.InvokeVoidAsync("mapHelper.destroyMap", _mapId);
        DialogService.Close(null);
    }

    private async Task Confirm()
    {
        await JSRuntime.InvokeVoidAsync("mapHelper.destroyMap", _mapId);
        DialogService.Close(_selectedLocation);
    }

    public class MapDataDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public void Dispose()
    {
        _dotnetRef?.Dispose();
    }
}