using Diplomski.RatingHub.Application.Models.Dtos;
using Diplomski.RatingHub.Application.UseCases.Categories.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace Diplomski.RatingHub.Web.Components.AdminPages.CategoryPages;

public partial class AddCategory
{
    [Parameter] public CategoryDto? ParentCategory { get; set; }
    
    [Inject] protected ICategoryDataService CategoryDataService { get; set; } = null!;

    protected CreateCategoryDto Model { get; set; } = new();

    protected RadzenDataGrid<CreateCategoryKeywordDto> KeywordsGrid = null!;
    protected RadzenDataGrid<CreateRatingCriterionDto> CriteriaGrid = null!;

    private bool IsKeywordNew = false;
    private CreateCategoryKeywordDto? originalKeyword;
    private bool IsCriterionNew = false;
    private CreateRatingCriterionDto? originalCriterion;

    protected async Task AddKeywordRow()
    {
        if (!KeywordsGrid.IsValid) return;
        
        var item = new CreateCategoryKeywordDto();
        Model.Keywords.Add(item);
        IsKeywordNew = true;
        
        await KeywordsGrid.InsertRow(item);
        
    }

    protected async Task EditKeyword(CreateCategoryKeywordDto item)
    {
        if (!KeywordsGrid.IsValid) return;

        originalKeyword = new CreateCategoryKeywordDto
        {
            Keyword = item.Keyword
        };
        
        await KeywordsGrid.EditRow(item);
    }

    protected async Task RemoveKeyword(CreateCategoryKeywordDto item)
    {
        Model.Keywords.Remove(item);
        await KeywordsGrid.Reload();
    }

    protected async Task SaveKeyword(CreateCategoryKeywordDto item)
    {
        if (!KeywordsGrid.IsValid) return;
        
        IsKeywordNew = false;
        await KeywordsGrid.UpdateRow(item);
    }

    protected void CancelKeywordEdit(CreateCategoryKeywordDto item)
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
        
        var item = new CreateRatingCriterionDto
        {
            IsActive = true
        };
        Model.RatingCriteria.Add(item);
        IsCriterionNew = true;
        
        await CriteriaGrid.InsertRow(item);
    }

    protected async Task EditCriterion(CreateRatingCriterionDto item)
    {
        if (!CriteriaGrid.IsValid) return;

        originalCriterion = new CreateRatingCriterionDto
        {
            Name = item.Name,
            SortOrder = item.SortOrder,
            IsActive = item.IsActive,
        };
        
        await CriteriaGrid.EditRow(item);
    }

    protected async Task RemoveCriterion(CreateRatingCriterionDto item)
    {
        Model.RatingCriteria.Remove(item);
        await KeywordsGrid.Reload();
    }

    protected async Task SaveCriterion(CreateRatingCriterionDto item)
    {
        if (!CriteriaGrid.IsValid) return;
        
        IsCriterionNew = false;
        await CriteriaGrid.UpdateRow(item);
        
    }

    protected void CancelCriterionEdit(CreateRatingCriterionDto item)
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
        
        // var dto = new CreateCategoryDto
        // {
        //     
        //     Name = Model.Name,
        //     Slug = Model.Slug,
        //     SortOrder = Model.SortOrder,
        //     ParentId = ParentCategory?.Id,
        //     Keywords = Model.Keywords
        //         .Where(x => !string.IsNullOrWhiteSpace(x.Keyword))
        //         .Select(x => new CreateCategoryKeywordDto
        //         {
        //             Keyword = x.Keyword.Trim()
        //         })
        //         .ToList(),
        //     RatingCriteria = Model.RatingCriteria
        //         .Where(x => !string.IsNullOrWhiteSpace(x.Name))
        //         .Select(x => new CreateRatingCriterionDto
        //         {
        //             Name = x.Name.Trim(),
        //             SortOrder = x.SortOrder,
        //             IsActive = x.IsActive
        //         })
        //         .ToList()
        // };
        Model.ParentId = ParentCategory?.Id;
        var result = await InvokeDataServiceMethod(() =>
                CategoryDataService.CreateCategory(Model), successMessage: "Uspesno ste kreirali kategoriju");
        if (!result) return;
        
        DialogService.Close(true);
    }
}