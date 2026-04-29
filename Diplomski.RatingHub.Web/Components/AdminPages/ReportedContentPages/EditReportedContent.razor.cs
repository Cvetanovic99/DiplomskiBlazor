using Diplomski.RatingHub.Application.UseCases.ReportedContents.Queries;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.AdminPages.ReportedContentPages;

public partial class EditReportedContent 
{
    [Parameter] public ReportedContentDto Model { get; set; }

    [Inject] protected IReportedContentDataService DataService { get; set; }

    private async Task Save()
    {
        await InvokeDataServiceMethod(() =>
                DataService.EditReportedContent(Model.Id, Model.Status),
            "Uspešno izmenjeno");

        DialogService.Close(true);
    }
    
    protected void Cancel()
    {
        DialogService.Close(false);
    }
}