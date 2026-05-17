using Diplomski.RatingHub.Domain.Enums;
using Diplomski.RatingHub.Web.Data.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.Shared;

public partial class ReportContentDialog 
{
    [Parameter] public ReportedContentEntityType ReportedEntityType { get; set; }
    [Parameter] public int ReportedEntityId { get; set; }
    [Parameter] public int? ReviewId { get; set; }
    [Parameter] public int? ContentOwnerId { get; set; }
    
    [Inject] public IReportedContentDataService ReportedContentDataService { get; set; } = null!;

    private ReportContentDto model = new();

    protected override void OnInitialized()
    {
        model.ReportedEntityType = ReportedEntityType;
        model.ReportedEntityId = ReportedEntityId;
        model.ContentOwnerId = ContentOwnerId;
        model.ReviewId = ReviewId;
    }

    private async Task OnSubmit()
    {
        var res = await InvokeDataServiceMethod(
            () => ReportedContentDataService.CreateReportedContent(model), 
            errorMessage: "Doslo je do greske prilikom prijavljivanja sadrzaja");
        
        if(res)
            DialogService.Close(true);
    }

    private void OnCancel()
    {
        DialogService.Close(false);
    }

    public class ReportContentDto
    {
        public string Title { get; set; } = "";
        public string Reason { get; set; } = "";
        public string? Email { get; set; }

        public ReportedContentEntityType ReportedEntityType { get; set; }
        public int? ReviewId { get; set; } //It will be used when response is reported, to display taht response for adnim together with review
        public int ReportedEntityId { get; set; }
        public int? ContentOwnerId { get; set; }
        public int? ReporterUserId { get; set; }
    }
}