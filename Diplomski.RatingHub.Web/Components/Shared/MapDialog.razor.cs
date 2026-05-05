using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Diplomski.RatingHub.Web.Components.Shared;

public partial class MapDialog
{
    [Parameter] public MapDataDto CityLocation { get; set; } = default!;
    [Parameter] public MapDataDto CompanyLocation { get; set; } = default!;

    private string _mapId = $"map-{Guid.NewGuid()}";

    private MapDataDto? _selectedLocation;
    private bool _hasSelection => _selectedLocation != null;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
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
                DotNetObjectReference.Create(this));
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

    private void Close()
    {
        DialogService.Close(null);
    }

    private void Confirm()
    {
        DialogService.Close(_selectedLocation);
    }

    public class MapDataDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}