using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Models;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace Diplomski.RatingHub.Web.Components.AdminPages.CategoryPages;

public partial class EditCategory
{
    [Parameter] public CategoryDto? Model { get; set; }
    
    [Inject] protected ICategoryDataService CategoryDataService { get; set; } = null!;

    protected RadzenDataGrid<CategoryKeywordDto> KeywordsGrid = null!;
    protected RadzenDataGrid<RatingCriterionDto> CriteriaGrid = null!;

    private bool IsKeywordNew = false;
    private CategoryKeywordDto? originalKeyword;
    private bool IsCriterionNew = false;
    private RatingCriterionDto? originalCriterion;

    protected async Task AddKeywordRow()
    {
        if (!KeywordsGrid.IsValid) return;
        
        var item = new CategoryKeywordDto();
        Model.Keywords.Add(item);
        IsKeywordNew = true;
        
        await KeywordsGrid.InsertRow(item);
        
    }

    protected async Task EditKeyword(CategoryKeywordDto item)
    {
        if (!KeywordsGrid.IsValid) return;

        originalKeyword = new CategoryKeywordDto
        {
            Keyword = item.Keyword
        };
        
        await KeywordsGrid.EditRow(item);
    }

    protected async Task RemoveKeyword(CategoryKeywordDto item)
    {
        Model.Keywords.Remove(item);
        await KeywordsGrid.Reload();
    }

    protected async Task SaveKeyword(CategoryKeywordDto item)
    {
        IsKeywordNew = false;
        await KeywordsGrid.UpdateRow(item);
    }

    protected void CancelKeywordEdit(CategoryKeywordDto item)
    {
        KeywordsGrid?.CancelEditRow(item);

        if (IsKeywordNew)
        {
            Model.Keywords.Remove(item);
            IsKeywordNew = false;
        }
        else
        {
            item.Keyword = originalKeyword.Keyword;
            originalKeyword = null;
        }
    }

    protected async Task AddRatingCriteriaRow()
    {if (!KeywordsGrid.IsValid) return;
        
        var item = new RatingCriterionDto
        {
            IsActive = true
        };
        Model.RatingCriteria.Add(item);
        IsCriterionNew = true;
        
        await CriteriaGrid.InsertRow(item);
    }

    protected async Task EditCriterion(RatingCriterionDto item)
    {
        if (!CriteriaGrid.IsValid) return;

        originalCriterion = new RatingCriterionDto
        {
            Name = item.Name,
            SortOrder = item.SortOrder,
            IsActive = item.IsActive,
        };
        
        await CriteriaGrid.EditRow(item);
    }

    protected async Task RemoveCriterion(RatingCriterionDto item)
    {
        Model.RatingCriteria.Remove(item);
        await KeywordsGrid.Reload();
    }

    protected async Task SaveCriterion(RatingCriterionDto item)
    {
        IsCriterionNew = false;
        await CriteriaGrid.UpdateRow(item);
        
    }

    protected void CancelCriterionEdit(RatingCriterionDto item)
    {
        CriteriaGrid?.CancelEditRow(item);

        if (IsCriterionNew)
        {
            Model.RatingCriteria.Remove(item);
            IsCriterionNew = false;
        }
        else
        {
            item.Name = originalCriterion.Name;
            item.SortOrder = originalCriterion.SortOrder;
            item.IsActive = originalCriterion.IsActive;
            
            originalCriterion = null;
        }
    }

    protected void Cancel()
    {
        DialogService.Close(false);
    }

    protected async Task SaveAsync()
    {
        var result = await InvokeDataServiceMethod(() =>
                CategoryDataService.EditCategory(Model), successMessage: "Uspesno ste azurirali kategoriju");
        if (!result) return;
        
        DialogService.Close(true);
    }
}