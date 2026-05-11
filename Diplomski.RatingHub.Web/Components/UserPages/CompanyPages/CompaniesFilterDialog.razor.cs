using Diplomski.RatingHub.Application.Enums;
using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.UserPages.CompanyPages;

public partial class CompaniesFilterDialog
{
    [Parameter] public CompaniesFilterDto InitialModel { get; set; }

    protected CompaniesFilterDto Model = new();

    protected IEnumerable<CompanyClaimStatusFilterOptions> _claimOptions = Enum.GetValues<CompanyClaimStatusFilterOptions>();
    protected IEnumerable<CompanyVerificationStatusFilterOptions> _verificationOptions = Enum.GetValues<CompanyVerificationStatusFilterOptions>();

    protected override void OnInitialized()
    {
        Model = InitialModel != null
            ? new CompaniesFilterDto
            {
                OverallAverageGrade = InitialModel.OverallAverageGrade,
                ClaimStatus = InitialModel.ClaimStatus,
                VerificationStatus = InitialModel.VerificationStatus
            }
            : new CompaniesFilterDto();
    }

    void OnApply()
    {
        DialogService.Close(Model); 
    }

    void OnCancel()
    {
        DialogService.Close(null);
    }
}

public class CompaniesFilterDto
{
    public double OverallAverageGrade { get; set; }
    public CompanyClaimStatusFilterOptions ClaimStatus { get; set; }
    public CompanyVerificationStatusFilterOptions VerificationStatus { get; set; }
}