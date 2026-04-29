using Diplomski.RatingHub.Application.UseCases.ReportedContents.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.AdminPages.ReportedContentPages;

public partial class ReportedContentDetails
{
    [Parameter] public ReportedContentDto Model { get; set; }

    [Inject] protected IReportedContentDataService DataService { get; set; }

    private bool _confirmDelete = false;

    private async Task Delete()
    {
        await InvokeDataServiceMethod(() =>
                DataService.DeleteReportedContent(Model.Id),
            "Uspešno obrisano");

        DialogService.Close(true);
    }
}