using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.HomePageSections;

public partial class PopularProvidersSection
{
    [Inject] public ICategoryDataService CategoryDataService { get; set; } = null!;

    private List<PopularCategoryDto> _popularCategories { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var response = await InvokeDataServiceMethod(
            () => CategoryDataService.GetPopularCategories(),
            errorMessage: "Greška prilikom ucitavanja kategorija");

        if(!response.ExceptionOccurred)
            _popularCategories = response.Result.ToList();
        // ProviderGroups =
        // [
        //     new ProviderGroup
        //     {
        //         Title = "Frizeri i saloni",
        //         Slug = "frizeri",
        //         Icon = "content_cut",
        //         Description = "Najcesce pregledani i najbolje ocenjeni saloni i frizeri.",
        //         Providers =
        //         [
        //             new ProviderCard { Id = "salon-bella", Name = "Salon Bella", Address = "Nis, Bulevar Nemanjica 24", Rating = 4.9m, IsCompany = true, Description = "Moderan frizerski salon za zene, muskarce i decu sa fokusom na negu i stilizovanje.", ImageUrl = "/images/companyImages/DSC_0319.JPG" },
        //             new ProviderCard { Id = "studio-glam", Name = "Studio Glam", Address = "Beograd, Bulevar kralja Aleksandra 112", Rating = 4.8m, IsCompany = true, Description = "Profesionalne usluge sisanja, feniranja, sminkanja i pripreme za posebne prilike.", ImageUrl = "/images/hero-handyman.jpg" },
        //             new ProviderCard { Id = "marko-styling", Name = "Marko Styling", Address = "Novi Sad, Jevrejska 8", Rating = 4.7m, IsCompany = false, Description = "Muski frizer specijalizovan za moderne fade frizure i precizno oblikovanje brade.", ImageUrl = "/images/companyImages/DSC_0323.JPG" },
        //             new ProviderCard { Id = "salon-mia", Name = "Salon Mia", Address = "Kragujevac, Kralja Petra I 17", Rating = 4.8m, IsCompany = true, Description = "Frizerski salon sa prijatnim ambijentom i kompletnom negom kose.", ImageUrl = "/images/providers/salon-mia.jpg" },
        //             new ProviderCard { Id = "nikola-cut", Name = "Nikola Cut", Address = "Nis, Obrenoviceva 35", Rating = 4.6m, IsCompany = false, Description = "Brza i kvalitetna usluga sisanja uz moderan pristup i iskustvo u radu.", ImageUrl = "/images/providers/nikola-cut.jpg" },
        //             new ProviderCard { Id = "beauty-lab", Name = "Beauty Lab", Address = "Beograd, Cara Dusana 42", Rating = 4.7m, IsCompany = true, Description = "Salon lepote sa frizerskim i kozmetickim tretmanima za svakodnevnu negu.", ImageUrl = "/images/providers/beauty-lab.jpg" }
        //         ]
        //     },
        //     new ProviderGroup
        //     {
        //         Title = "Kucne usluge",
        //         Slug = "kucne-usluge",
        //         Icon = "home_repair_service",
        //         Description = "Pouzdani majstori za kvarove, popravke i radove u stanu i kuci.",
        //         Providers =
        //         [
        //             new ProviderCard { Id = "milan-vodoinstalater", Name = "Milan Petrovic", Address = "Nis, Durlan", Rating = 4.9m, IsCompany = false, Description = "Vodoinstalaterske usluge, hitne intervencije i sanacija curenja i kvarova.", ImageUrl = "/images/hero-hairdresser.jpg" },
        //             new ProviderCard { Id = "elektro-max", Name = "Elektro Max", Address = "Beograd, Zvezdara", Rating = 4.7m, IsCompany = true, Description = "Elektricarske usluge, popravke instalacija i zamena osiguraca i rasvete.", ImageUrl = "/images/hero-handyman.jpg" },
        //             new ProviderCard { Id = "brzi-servis", Name = "Brzi Servis", Address = "Novi Sad, Detelinara", Rating = 4.8m, IsCompany = true, Description = "Kucne popravke i razne intervencije za stanove, kuce i poslovne prostore.", ImageUrl = "/images/companyImages/2d760657-74e4-4214-817c-79aaabfb9fdf.jpg" },
        //             new ProviderCard { Id = "dejan-elektricar", Name = "Dejan Jovanovic", Address = "Leskovac, Centar", Rating = 4.6m, IsCompany = false, Description = "Iskusan elektricar za manje i vece radove, od uticnica do kompletnih instalacija.", ImageUrl = "/images/providers/dejan-elektricar.jpg" },
        //             new ProviderCard { Id = "keramika-plus", Name = "Keramika Plus", Address = "Kraljevo, Dositejeva 11", Rating = 4.7m, IsCompany = true, Description = "Postavljanje keramike, renoviranje kupatila i zavrsni gradjevinski radovi.", ImageUrl = "/images/providers/keramika-plus.jpg" },
        //             new ProviderCard { Id = "dom-majstor", Name = "Dom Majstor", Address = "Kragujevac, Erdoglija", Rating = 4.8m, IsCompany = true, Description = "Sve vrste sitnih i srednjih kucnih popravki uz brz izlazak na teren.", ImageUrl = "/images/providers/dom-majstor.jpg" }
        //         ]
        //     },
        //     new ProviderGroup
        //     {
        //         Title = "Auto servisi",
        //         Slug = "auto-servisi",
        //         Icon = "directions_car",
        //         Description = "Popularni servisi i automehanicari sa dobrim ocenama korisnika.",
        //         Providers =
        //         [
        //             new ProviderCard { Id = "auto-centar", Name = "Auto Centar", Address = "Beograd, Mirijevo", Rating = 4.8m, IsCompany = true, Description = "Servisiranje i dijagnostika vozila svih marki, uz redovno odrzavanje i popravke.", ImageUrl = "/images/providers/auto-centar.jpg" },
        //             new ProviderCard { Id = "goran-mehanicar", Name = "Goran Stojanovic", Address = "Nis, Pantelej", Rating = 4.7m, IsCompany = false, Description = "Automehanicar sa iskustvom u servisiranju motora, kocnica i trapova.", ImageUrl = "/images/providers/goran-mehanicar.jpg" },
        //             new ProviderCard { Id = "servis-delta", Name = "Servis Delta", Address = "Novi Sad, Temerinska", Rating = 4.9m, IsCompany = true, Description = "Kompletan auto servis sa modernom opremom i brzom dijagnostikom kvarova.", ImageUrl = "/images/providers/servis-delta.jpg" },
        //             new ProviderCard { Id = "vulkanizer-lux", Name = "Vulkanizer Lux", Address = "Kragujevac, Centar", Rating = 4.6m, IsCompany = true, Description = "Zamena i balansiranje guma, vulkanizerske i manje servisne usluge.", ImageUrl = "/images/providers/vulkanizer-lux.jpg" },
        //             new ProviderCard { Id = "mika-auto", Name = "Mika Auto", Address = "Subotica, Sencanski put", Rating = 4.7m, IsCompany = false, Description = "Auto elektrika, dijagnostika i popravke elektronskih komponenti na vozilu.", ImageUrl = "/images/providers/mika-auto.jpg" },
        //             new ProviderCard { Id = "garage-pro", Name = "Garage Pro", Address = "Nis, Industrijska zona", Rating = 4.8m, IsCompany = true, Description = "Pouzdan servis za mehaniku, mali servis i pregled vozila pre putovanja.", ImageUrl = "/images/providers/garage-pro.jpg" }
        //         ]
        //     }
        // ];
    }

    protected IEnumerable<List<PopularCompanyDto>> ChunkProviders(List<PopularCompanyDto> providers, int size)
    {
        for (int i = 0; i < providers.Count; i += size)
        {
            yield return providers.Skip(i).Take(size).ToList();
        }
    }

    private void NavigateToCompany(PopularCompanyDto company)
    {
        NavigationManager.NavigateTo($"/companies/{company.Id}");
    }

    private async Task AllFromCategoryClicked(PopularCategoryDto category)
    {
        var result = await DialogService.OpenAsync<SelectCityDialog>(
            "Izbor grada",
            options: new DialogOptions
            {
                Width = "500px",
                Height = "auto",
                Style = "margin-top: 130px"
            });

        if (result != null)
        {
            int cityId = (int)result;
            NavigationManager.NavigateTo($"/companies?CityId={cityId}&CategoryId={category.Id}");
        }
    }

    private string GetStarsFillStyle(double rating)
    {
        var percentage = Math.Clamp((double)(rating / 5.0) * 100d, 0d, 100d);
        return $"--stars-fill:{percentage:F2}%";
    }

    protected class ProviderGroup
    {
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<ProviderCard> Providers { get; set; } = [];
    }

    protected class ProviderCard
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public bool IsCompany { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}