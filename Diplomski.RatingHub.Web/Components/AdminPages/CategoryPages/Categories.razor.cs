using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Utilities;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Diplomski.RatingHub.Web.Components.AdminPages.CategoryPages;

public partial class Categories
{
    [Inject] protected ICategoryDataService CategoryDataService { get; set; } = null!;
    
    
    private string _filterValue = string.Empty;
    private RadzenDataGrid<CategoryDto> _categoriesGrid;
    private IEnumerable<CategoryDto>? _categories;
    private int _categoriesCount;
    
    private RadzenDataGrid<NewCategorySuggestionDto> _categorySuggestionsGrid;
    protected IEnumerable<NewCategorySuggestionDto> _categorySuggestions;
    private int _categoriySuggestionsCount;
    private NewCategorySuggestionStatus? _selectedSuggestionStatus;
    
    protected async Task LoadCategories(LoadDataArgs args)
    {
        if(string.IsNullOrEmpty(args.OrderBy))
            args.OrderBy = "Id asc";
        
        var response = await InvokeDataServiceMethod(() =>
            CategoryDataService.GetCategories(_filterValue.Trim(), args.ToQueryArgs()));
        _categories = response.Result?.Items;
        _categoriesCount = response.Result.TotalCount;
    }

    protected async Task LoadCategorySuggestions(LoadDataArgs args)
    {
        if(string.IsNullOrEmpty(args.OrderBy))
            args.OrderBy = "Created desc";
        
        // Convert QueryArgs to include the Status filter from the dropdown
        var queryArgs = args.ToQueryArgs();
        
        // Build combined filter expression for enum comparison with existing filters
        string statusFilter = _selectedSuggestionStatus.HasValue 
            ? $"(x.Status == {(int)_selectedSuggestionStatus.Value})"
            : null;
        
        // Combine existing filter with status filter
        if (!string.IsNullOrWhiteSpace(queryArgs.Filter) && !string.IsNullOrWhiteSpace(statusFilter))
        {
            // Both filters exist - combine them with AND
            // Remove "x => " from existing filter, add status filter, then add "x => " back
            var existingFilterBody = queryArgs.Filter.StartsWith("x =>") 
                ? queryArgs.Filter.Substring(5).Trim() 
                : queryArgs.Filter;
            
            queryArgs.Filter = $"x => ({existingFilterBody}) && {statusFilter}";
        }
        else if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            // Only status filter exists
            queryArgs.Filter = $"x => {statusFilter}";
        }
        // else: use existing filter or no filter if status is not selected
        
        var response = await InvokeDataServiceMethod(() =>
            CategoryDataService.GetNewCategorySuggestions(queryArgs));
        
        _categorySuggestions = response.Result?.Items;
        _categoriySuggestionsCount = response.Result.TotalCount;
    }

    private async Task Search(ChangeEventArgs args)
    {
        _filterValue = $"{args.Value}";

        await _categoriesGrid.GoToPage(0, true);
    }

    protected async Task OnSuggestionStatusFilterChanged()
    {
        // Reset to first page when filter changes and reload
        if (_categorySuggestionsGrid != null)
            await _categorySuggestionsGrid.GoToPage(0, true);
    }

    private async Task OpenAddCategoryDialog()
    {
        var result = await DialogService.OpenAsync<AddCategory>(
            "Kreiranje Kategorije",
            new Dictionary<string, object?>
            {
                { "ParentCategory", null }
            },
            options: new DialogOptions
            {
                Width = "70%;",
                Height = "75%",
                Style = "margin-top: 130px",
                CloseDialogOnOverlayClick = true
            });

        if (result is true)
            await _categoriesGrid.Reload();
    }
    
    private async Task OpenAddSubcategoryDialog(CategoryDto item)
    {
        var result = await DialogService.OpenAsync<AddCategory>(
            "Kreiranje podkategorije",
            new Dictionary<string, object?>
            {
                { "ParentCategory", item }
            },
            options: new DialogOptions
            {
                Width = "70%;",
                Height = "75%",
                Style = "margin-top: 130px",
                CloseDialogOnOverlayClick = true
            });

        if (result is true)
            await _categoriesGrid.Reload();
    }

    protected async Task OpenEditCategoryDialog(CategoryDto item)
    {
        var result = await DialogService.OpenAsync<EditCategory>(
            "Azuriranje Kategorije",
            new Dictionary<string, object?>
            {
                { "Model", item }
            },
            options: new DialogOptions
            {
                Width = "70%;",
                Height = "75%",
                Style = "margin-top: 130px",
                CloseDialogOnOverlayClick = true
            });

        if (result is true)
            await _categoriesGrid.Reload();
    }

    protected async Task DeleteCategory(CategoryDto item)
    {
        var confirmed = await DialogService.Confirm(
            $"Da li ste sigurni da zelite da obrisete kategoriju '{item.Name}'?",
            "Potvrda brisanja",
            new ConfirmOptions { OkButtonText = "Obrisi", CancelButtonText = "Odustani" });

        if (confirmed is true)
        {
            var response = await InvokeDataServiceMethod(() =>
                CategoryDataService.DeleteCategory(item.Id),"Uspesno ste obrisali kategoriju");

            if (response)
                await _categoriesGrid.Reload();
        }
    }

    protected async Task OpenEditSuggestionDialog(NewCategorySuggestionDto item)
    {
        var result = await DialogService.OpenAsync<EditNewCategorySuggestion>(
            "Azuriranje Statusa Predloga",
            new Dictionary<string, object?>
            {
                { "Model", item }
            },
            options: new DialogOptions
            {
                Width = "400px",
                Height = "auto",
                Style = "margin-top: 130px",
                CloseDialogOnOverlayClick = true
            });

        if (result is true)
            await _categorySuggestionsGrid.Reload();
    }

    protected async Task DeleteSuggestion(NewCategorySuggestionDto item)
    {
        var confirmed = await DialogService.Confirm(
            $"Da li ste sigurni da zelite da obrisete predlog za kategoriju '{item.Name}'?",
            "Potvrda brisanja",
            new ConfirmOptions { OkButtonText = "Obrisi", CancelButtonText = "Odustani" });

        if (confirmed is true)
        {
            var response = await InvokeDataServiceMethod(() =>
                CategoryDataService.DeleteCategoryNewSuggestion(item.Id),"Uspesno ste obrisali predlog kategorije");

            if (response)
                await _categorySuggestionsGrid.Reload();
        }
    }

    protected string GetStatusCss(string status) =>
        status?.ToLower() switch
        {
            "na cekanju" => "status-pending",
            "odobren" => "status-approved",
            "odbijen" => "status-rejected",
            _ => "status-default"
        };
    
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