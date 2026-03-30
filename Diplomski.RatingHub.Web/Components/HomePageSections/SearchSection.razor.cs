using Microsoft.AspNetCore.Components;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.HomePageSections;

public partial class SearchSection : ComponentBase
{
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    [Parameter] public string SectionId { get; set; } = "home-search-section";

    protected string? SearchTerm { get; set; }
    protected string? SelectedCity { get; set; }

    protected List<SearchSuggestionItem> Suggestions { get; set; } = new();
    protected List<SearchSuggestionItem> FilteredSuggestions { get; set; } = new();

    protected List<string> Cities { get; set; } = new();
    protected List<string> FilteredCities { get; set; } = new();

    protected override void OnInitialized()
    {
        Suggestions =
        [
            new() { Label = "Frizeri", Subtitle = "Kategorija" },
            new() { Label = "Muški frizeri", Subtitle = "Podkategorija" },
            new() { Label = "Salon Bella", Subtitle = "Firma · Niš" },
            new() { Label = "Salon Glamour", Subtitle = "Firma · Beograd" },
            new() { Label = "Vodoinstalateri", Subtitle = "Kategorija" },
            new() { Label = "Električari", Subtitle = "Kategorija" },
            new() { Label = "Auto servis", Subtitle = "Kategorija" },
            new() { Label = "Marko Jovanović", Subtitle = "Majstor · Električar" },
            new() { Label = "Milan Petrović", Subtitle = "Majstor · Vodoinstalater" }
        ];

        Cities =
        [
            "Niš",
            "Beograd",
            "Novi Sad",
            "Kragujevac",
            "Subotica",
            "Pančevo",
            "Čačak",
            "Leskovac",
            "Kraljevo"
        ];

        FilteredSuggestions = Suggestions;
        FilteredCities = Cities;
    }

    protected void LoadSuggestions(LoadDataArgs args)
    {
        var term = args.Filter?.Trim();

        FilteredSuggestions = string.IsNullOrWhiteSpace(term)
            ? Suggestions
            : Suggestions
                .Where(x =>
                    x.Label.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    x.Subtitle.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
    }

    protected void LoadCities(LoadDataArgs args)
    {
        var term = args.Filter?.Trim();

        FilteredCities = string.IsNullOrWhiteSpace(term)
            ? Cities
            : Cities
                .Where(x => x.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
    }

    protected void OnSearchTermChanged(object? value)
    {
        SearchTerm = value?.ToString();
    }

    protected void OnCityChanged(object? value)
    {
        SelectedCity = value?.ToString();
    }

    protected void ExecuteSearch()
    {
        var query = new Dictionary<string, object?>();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
            query["term"] = SearchTerm;

        if (!string.IsNullOrWhiteSpace(SelectedCity))
            query["city"] = SelectedCity;

        var url = Navigation.GetUriWithQueryParameters("/search", query);
        Navigation.NavigateTo(url);
    }

    protected class SearchSuggestionItem
    {
        public string Label { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
    }
}