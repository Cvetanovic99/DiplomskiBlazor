using Diplomski.RatingHub.Application.Models;
using Diplomski.RatingHub.Application.UseCases.CompanyVerifications.Queries;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Diplomski.RatingHub.Web.Components.AdminPages.VerificationRequestPages;

public partial class CompanyVerificationRequests
{
    [Inject] protected ICompanyVerificationRequestDataService DataService { get; set; }

    protected RadzenDataGrid<CompanyVerificationRequestDto> _grid;

    protected IEnumerable<CompanyVerificationRequestDto> _data;
    protected int _count;

    protected string _search = "";
    protected CompanyVerificationRequestStatus? _status;

    protected async Task LoadData(LoadDataArgs args)
    {
        var queryArgs = new QueryArgs
        {
            Skip = args.Skip ?? 0,
            Take = args.Top ?? 20
        };

        var result = await InvokeDataServiceMethod(() =>
            DataService.GetVerificationRequests(_search, _status, queryArgs));

        _data = result.Result.Items;
        _count = result.Result.TotalCount;
    }

    protected async Task Search(ChangeEventArgs args)
    {
        _search = args.Value?.ToString().ToLower();
        await _grid.Reload();
    }

    protected async Task OnFilterChanged(object value)
    {
        _status = (CompanyVerificationRequestStatus?)value;
        await _grid.Reload();
    }

    private async Task OpenDetails(CompanyVerificationRequestDto item)
    {
        await DialogService.OpenAsync<CompanyVerificationRequestDetails>(
            "Detalji Zahteva Verifikacije",
            new Dictionary<string, object?>
            {
                { "Model", item }
            },
            options: new DialogOptions
            {
                Width = "70%",
                Height = "70%",
                Style = "margin-top: 130px",
                CloseDialogOnOverlayClick = true
            });
        
    }

    private async Task Delete(CompanyVerificationRequestDto item)
    {
        var confirm = await DialogService.Confirm("Obrisati zahtev?", "Potvrda", new ConfirmOptions
        {
            OkButtonText = "Obrisi",
            CancelButtonText = "Odustani",
        });

        if (confirm == true)
        {
            var response = await InvokeDataServiceMethod(() =>
                    DataService.DeleteVerificationRequest(item.Id), "Uspešno obrisano");

            if (response)
                await _grid.Reload();
        }
    }
    
    private string GetStatusOptions(object value)
    {
        var option = (CompanyVerificationRequestStatus)value;
        switch (option)
        {
            case CompanyVerificationRequestStatus.Dismissed:
                return "Odbijen";
            case CompanyVerificationRequestStatus.Pending: 
                return "Na cekanju";
            case CompanyVerificationRequestStatus.AcctionTaken:
                return "Obradjuje se";
            case CompanyVerificationRequestStatus.Approved:
                return "Prihvacen";
            default:
                return "";
        }
    }
}