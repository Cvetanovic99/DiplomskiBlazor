using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Data.Services;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyPages;

public partial class SubcategoriesDialog 
{
    [Parameter] public int ParentCategoryId { get; set; }
    [Inject] public ICategoryDataService CategoryDataService { get; set; }
    

    private IEnumerable<SubcategoryDto> _subcategories = new List<SubcategoryDto>();

    protected override async Task OnInitializedAsync()
    {
        var response = await InvokeDataServiceMethod(
            () => CategoryDataService.GetSubcategories(ParentCategoryId),
            errorMessage: "Greška pri učitavanju podkategorija");

        if (!response.ExceptionOccurred)
        {
            _subcategories = _subcategories = new List<SubcategoryDto>
            {
                new SubcategoryDto { Id = 1, Name = "Podkategorija 1" },
                new SubcategoryDto{ Id = 2, Name = "Podkategorija 2" },
                new SubcategoryDto { Id = 3, Name = "Podkategorija 3", },
                new SubcategoryDto { Id = 4, Name = "Podkategorija 4" },
                new SubcategoryDto{ Id = 5, Name = "Podkategorija 5"},
                new SubcategoryDto{ Id = 6, Name = "Podkategorija 6" },
                new SubcategoryDto{ Id = 7, Name = "Podkategorija 7" },
                new SubcategoryDto{ Id = 8, Name = "Podkategorija 8" },
                new SubcategoryDto{ Id = 9, Name = "Podkategorija 9" } ,
                new SubcategoryDto { Id = 3, Name = "Podkategorija 3", },
                new SubcategoryDto { Id = 3, Name = "Podkategorija 3", },
                new SubcategoryDto { Id = 3, Name = "Podkategorija 3", },
                new SubcategoryDto { Id = 3, Name = "Podkategorija 3", },
                new SubcategoryDto { Id = 3, Name = "Podkategorija 3", },
                new SubcategoryDto { Id = 3, Name = "Podkategorija 3", },
                new SubcategoryDto { Id = 3, Name = "Podkategorija 3", },
                new SubcategoryDto { Id = 3, Name = "Podkategorija 3", },
                new SubcategoryDto { Id = 3, Name = "Podkategorija 3", },
            };//response.Result.Items;
        }
    }

    void SelectSubcategory(int id)
    {
        DialogService.Close(id);
    }

    void OnCancel()
    {
        DialogService.Close(null);
    }
}