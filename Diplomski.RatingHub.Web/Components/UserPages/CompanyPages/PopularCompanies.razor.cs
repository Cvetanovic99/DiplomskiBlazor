using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyPages;

public partial class PopularCompanies 
{
    [Parameter] public int CityId { get; set; }
    [Parameter] public int CategoryId { get; set; }
    
    [Inject] public ICompanyDataService CompanyDataService { get; set; }

    private List<PopularCompanyDto> Companies = new List<PopularCompanyDto>();

    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            if (CityId == 0 || CategoryId == 0)
            {
                ShowNotification("Doslo je do greske", NotificationSeverity.Error);
                return;
            }
    
            var res = await InvokeDataServiceMethod(
                () => CompanyDataService.GetPopularCompanies(CityId, CategoryId, 10),
                errorMessage: "Greška pri učitavanju kompanija");
    
            if (!res.ExceptionOccurred)
                Companies = res.Result.ToList();
        }
    }

    private IEnumerable<List<PopularCompanyDto>> ChunkCompanies(List<PopularCompanyDto> source, int size)
    {
        for (int i = 0; i < source.Count; i += size)
            yield return source.Skip(i).Take(size).ToList();
    }

    private string GetStarsFillStyle(double rating)
    {
        var percent = (rating / 5.0) * 100;
        return $"--stars-fill: {percent}%";
    }
    
    private void GoToDetails(int companyId)
    {
        NavigationManager.NavigateTo($"/companies/{companyId}");
    }
}