using Diplomski.RatingHub.Application.UseCases.ReportedContents.Queries;
using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Diplomski.RatingHub.Web.Utilities;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace Diplomski.RatingHub.Web.Components.AdminPages.ReportedContentPages;

public partial class ReportedContent 
{
    [Inject] protected IReportedContentDataService DataService { get; set; }

    private RadzenDataGrid<ReportedContentDto> _grid;
    private IEnumerable<ReportedContentDto> _data;
    private int _count;

    private string _search = string.Empty;
    private ReportedContentEntityType? _selectedType;
    private ReportedContentStatus? _selectedStatus;

    protected async Task LoadData(LoadDataArgs args)
    {
        if (string.IsNullOrEmpty(args.OrderBy))
            args.OrderBy = "Created desc";

        var response = await InvokeDataServiceMethod(() =>
            DataService.GetReportedContents(_search, _selectedType, _selectedStatus, args.ToQueryArgs()));

        _data = response.Result.Items;
        _count = response.Result.TotalCount;
    }

    private async Task Search(ChangeEventArgs e)
    {
        _search = e.Value?.ToString() ?? "";
        await _grid.GoToPage(0, true);
    }

    private async Task OnFilterChanged(object value)
    {
        await _grid.GoToPage(0, true);
    }
    private async Task OpenDetails(ReportedContentDto item)
    {
        await DialogService.OpenAsync<ReportedContentDetails>(
            "Detalji",
            new Dictionary<string, object?>
            {
                { "Model", item }
            },
            new DialogOptions { Width = "500px", Style = "margin-top: 130px"});
    }

    private async Task OpenEdit(ReportedContentDto item)
    {
        var result = await DialogService.OpenAsync<EditReportedContent>(
            "Izmena",
            new Dictionary<string, object?>
            {
                { "Model", item }
            },
            new DialogOptions { Width = "400px", Style = "margin-top: 130px"});

        if (result == true)
            await _grid.Reload();
    }

    private async Task Delete(ReportedContentDto item)
    {
        var confirm = await DialogService.Confirm("Da li želite da obrišete prijavu?", "Potvrda", 
            new ConfirmOptions
        {
            OkButtonText = "Obrisi",
            CancelButtonText = "Odustani",
        });

        if (confirm is true)
        {
            await InvokeDataServiceMethod(() =>
                    DataService.DeleteReportedContent(item.Id),
                "Uspešno obrisano");

            await _grid.Reload();
        }
    }
}