using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.AdminPages.CategoryPages;

public partial class EditNewCategorySuggestion
{
    [Parameter] public NewCategorySuggestionDto Model { get; set; }
    
    [Inject] protected ICategoryDataService CategoryDataService { get; set; } = null!;
    
    protected async Task SaveAsync()
    {
        var result = await InvokeDataServiceMethod(() =>
            CategoryDataService.EditNewCategorySuggestion(Model), successMessage: "Uspesno ste azurirali status");
        
        if (!result) return;
        
        DialogService.Close(true);
    }
    
    protected void Cancel()
    {
        DialogService.Close(false);
    }
    
    private string GetStatusOptions(object value)
    {
        var option = (NewCategorySuggestionStatus)value;
        switch (option)
        {
            case NewCategorySuggestionStatus.Dismissed:
                return "Odbijen";
            case NewCategorySuggestionStatus.Pending: 
                return "Na cekanju";
            case NewCategorySuggestionStatus.Approved:
                return "Prihvacen";
            default:
                return "";
        }
    }
}