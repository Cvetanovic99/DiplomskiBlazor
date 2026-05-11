using Diplomski.RatingHub.Application.UseCases.Companies.Queries;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyPages;

public partial class CompanyCard 
{
    [Parameter] public FilteredCompanyDto Company { get; set; } = default!;
    
    private string GetMainImage()
    {
        return string.IsNullOrEmpty(Company.ProfileImagePath)
            ? "/images/companyImages/genericCompanyImage.svg"
            : Company.ProfileImagePath;
    }

    private void GoToDetails()
    {
        NavigationManager.NavigateTo($"/companies/{Company.Id}");
    }

    private string GetStarsFillStyle(double rating)
    {
        var percentage = (rating / 5.0) * 100;
        return $"--stars-fill: {percentage}%";
    }
}