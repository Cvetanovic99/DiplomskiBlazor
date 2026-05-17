using Diplomski.RatingHub.Application.Enums;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.Shared;

public partial class AnonymousEditContentDialog 
{
    [Parameter] public AnonymousEditContentType ContentType { get; set; }
    [Parameter] public bool IsEdit { get; set; }
    [Parameter] public string Text { get; set; }
    [Parameter] public int  EntityId { get; set; }

    [Inject] public ICompanyDataService CompanyDataService { get; set; } = null!;
    [Inject] public IReviewDataService ReviewDataService { get; set; } = null!;

    private ModelDto model = new();
    private string _text = string.Empty;
    private string _textColor = "#353334";
    private string _submiButtonText = string.Empty;
    private string _submiButtonBusyText = string.Empty;

    override protected void OnInitialized()
    {
        _text = Text;
        _submiButtonText = IsEdit ? "Potvrdi" : "Izbrisi";
        _submiButtonBusyText = IsEdit ? "Potvrdjivanje" : "Brisanje";
    }

    private async Task OnSubmit()
    {
        if (ContentType == AnonymousEditContentType.Company)
        {
            var res = await InvokeDataServiceMethod(
                () => CompanyDataService.ValidateCompanyAnonymousEditIdentifier(EntityId, model.Code.Trim()),
                errorMessage: "Doslo je do greske prilikom validacije koda");
            
            if (!res.ExceptionOccurred)
            {
                if (res.Result)
                {
                    if (IsEdit)
                    {
                        DialogService.Close(true);
                    }
                    else
                    {
                        await DeleteCompany();
                    }
                }
                else
                {
                    if(IsEdit)
                        _text = "Vas kod za azuriranje ove kompanije nije ispravan";
                    else
                        _text = "Vas kod za brisanje ove kompanije nije ispravan";
                    
                    _textColor = "#c13f3f";
                }
            }
        }
        else if(ContentType == AnonymousEditContentType.Review)
        {
            var res = await InvokeDataServiceMethod(
                () => ReviewDataService.ValidateReviewAnonymousEditIdentifier(EntityId, model.Code.Trim()),
                errorMessage: "Doslo je do greske prilikom validacije koda");
            
            if (!res.ExceptionOccurred)
            {
                if (res.Result)
                {
                    if (IsEdit)
                    {
                        DialogService.Close(true);
                    }
                    else
                    {
                        await DeleteReview();
                    }
                }
                else
                {
                    if(IsEdit)
                        _text = "Vas kod za azuriranje ocene nije ispravan";
                    else
                        _text = "Vas kod za brisanje ove ocene nije ispravan";
                    
                    _textColor = "#c13f3f";
                }
            }
        }
    }

    private async Task DeleteCompany()
    {
        var res = await InvokeDataServiceMethod(
            () => CompanyDataService.DeleteCompanyAsAnonymous(EntityId),
            errorMessage: "Doslo je do greske prilikom brisanja kompanije");
        
        if (res)
        {
            DialogService.Close(true); 
        }
    }
    
    private async Task DeleteReview()
    {
        var res = await InvokeDataServiceMethod(
            () => ReviewDataService.DeleteReview(EntityId),
            errorMessage: "Doslo je do greske prilikom brisanja ocene");
        if (res)
        {
            DialogService.Close(true);
        }
    }

    private void OnCancel()
    {
        DialogService.Close(false);
    }

    private class ModelDto
    {
        public string Code { get; set; } = string.Empty;
    }
}