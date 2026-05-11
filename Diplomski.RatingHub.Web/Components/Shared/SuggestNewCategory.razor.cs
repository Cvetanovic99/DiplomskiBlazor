using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.Shared;

public partial class SuggestNewCategory
{
    [Parameter] public int? ParentCategoryId { get; set; }
    [Inject] public ICategoryDataService CategoryDataService { get; set; }

    private SuggestCategoryModel _model = new();
    

    private async Task OnSubmit(SuggestCategoryModel model)
    {
        _model.ParentCategoryId = ParentCategoryId;
        
        var response = await InvokeDataServiceMethod(
            () => CategoryDataService.CreateNewCategorySuggestion(_model),
            errorMessage: "Greška pri učitavanju podkategorija");
        
        if (response)
            DialogService.Close(true);
    }

    private void OnCancel()
    {
        DialogService.Close(false);
    }
}

public class SuggestCategoryModel
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
}