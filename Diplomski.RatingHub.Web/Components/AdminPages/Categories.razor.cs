using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Utilities;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Diplomski.RatingHub.Web.Components.AdminPages;

public partial class Categories
{
    [Inject] protected ICategoryDataService CategoryDataService { get; set; } = null!;

    private string _categorySearchTerm { get; set; } = string.Empty;

    protected List<CategoryRowVm> CategoriesList { get; set; } = new();
    protected List<CategorySuggestionRowVm> Suggestions { get; set; } = new();
    
    private RadzenDataGrid<CategoryRowVm> _grid;
    
    private string _filterValue = string.Empty;
    private RadzenDataGrid<CategoryDto> _categoriesGrid;
    private IEnumerable<CategoryDto>? _categories;
    private int _categoriesCount;
    

    protected async Task LoadCategories(LoadDataArgs args)
    {
        if(string.IsNullOrEmpty(args.OrderBy))
            args.OrderBy = "Id asc";
        
        var response = await InvokeDataServiceMethod(() =>
            CategoryDataService.GetCategories(_filterValue.Trim(), args.ToQueryArgs()));
        _categories = response.Result?.Items;
        _categoriesCount = response.Result.TotalCount;
    }
    
    private async Task Search(ChangeEventArgs args)
    {
        _filterValue = $"{args.Value}";

        await _categoriesGrid.GoToPage(0, true);
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
                Width = "900px;",
                Height = "700px",
                Style = "margin-top: 130px",
                CloseDialogOnOverlayClick = true
            });

        if (result is true)
            await _categoriesGrid.Reload();
    }

    protected async Task OpenEditCategoryDialog(CategoryDto item)
    {
        var model = new CategoryDto
        {
            Id = item.Id,
            Name = item.Name,
            Slug = item.Slug,
            Keywords = item.Keywords
        };

        // var result = await DialogService.OpenAsync<CategoryDialog>(
        //     "Izmena kategorije",
        //     new Dictionary<string, object?>
        //     {
        //         { "Model", model },
        //         { "IsEdit", true }
        //     },
        //     new DialogOptions
        //     {
        //         Width = "560px",
        //         CloseDialogOnOverlayClick = true,
        //         ShowClose = true,
        //         Resizable = false,
        //         Draggable = false
        //     });
        //
        // if (result is CategoryUpsertVm updatedModel)
        // {
        //     await CategoryAdminService.UpdateCategoryAsync(updatedModel);
        //     ShowSuccess("Kategorija je uspesno izmenjena.");
        //     await LoadData();
        // }
    }

    protected async Task DeleteCategory(CategoryDto item)
    {
        var confirmed = await DialogService.Confirm(
            $"Da li ste sigurni da zelite da obrisete kategoriju '{item.Name}'?",
            "Potvrda brisanja",
            new ConfirmOptions { OkButtonText = "Obrisi", CancelButtonText = "Odustani" });

        // if (confirmed == true)
        // {
        //     await CategoryAdminService.DeleteCategoryAsync(item.Id);
        //     ShowSuccess("Kategorija je uspesno obrisana.");
        //     await LoadData();
        // }
    }

    protected async Task OpenEditSuggestionDialog(CategorySuggestionRowVm item)
    {
        var model = new CategorySuggestionEditVm
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Status = item.Status
        };

        // var result = await DialogService.OpenAsync<CategorySuggestionDialog>(
        //     "Izmena predloga",
        //     new Dictionary<string, object?>
        //     {
        //         { "Model", model }
        //     },
        //     new DialogOptions
        //     {
        //         Width = "560px",
        //         CloseDialogOnOverlayClick = true,
        //         ShowClose = true,
        //         Resizable = false,
        //         Draggable = false
        //     });

        // if (result is CategorySuggestionEditVm updatedModel)
        // {
        //     await CategoryAdminService.UpdateSuggestionAsync(updatedModel);
        //     ShowSuccess("Predlog je uspesno izmenjen.");
        //     await LoadData();
        // }
    }

    protected async Task DeleteSuggestion(CategorySuggestionRowVm item)
    {
        var confirmed = await DialogService.Confirm(
            $"Da li ste sigurni da zelite da obrisete predlog '{item.Name}'?",
            "Potvrda brisanja",
            new ConfirmOptions { OkButtonText = "Obrisi", CancelButtonText = "Odustani" });

        // if (confirmed == true)
        // {
        //     await CategoryAdminService.DeleteSuggestionAsync(item.Id);
        //     ShowSuccess("Predlog je uspesno obrisan.");
        //     await LoadData();
        // }
    }

    protected string GetStatusCss(string status) =>
        status?.ToLower() switch
        {
            "na cekanju" => "status-pending",
            "odobren" => "status-approved",
            "odbijen" => "status-rejected",
            _ => "status-default"
        };

    private void ShowSuccess(string message)
    {
        NotificationService.Notify(new NotificationMessage
        {
            Severity = NotificationSeverity.Success,
            Summary = "Uspeh",
            Detail = message,
            Duration = 3000
        });
    }

    public class CategoryRowVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CompaniesCount { get; set; }
        public IEnumerable<string> Keywords { get; set; } = Enumerable.Empty<string>();

        public string KeywordsDisplayFull => string.Join(", ", Keywords);

        public string KeywordsDisplayShort
        {
            get
            {
                var full = string.Join(", ", Keywords);
                return full.Length <= 45 ? full : $"{full[..45]}...";
            }
        }
    }

    public class CategorySuggestionRowVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CategoryUpsertVm
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Keywords { get; set; } = new();
    }

    public class CategorySuggestionEditVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}