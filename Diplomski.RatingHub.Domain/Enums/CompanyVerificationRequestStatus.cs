using System.ComponentModel.DataAnnotations;

namespace Diplomski.RatingHub.Domain.Enums;

public enum CompanyVerificationRequestStatus
{
    Pending = 0,
    AcctionTaken = 1,
    Approved = 2,
    Dismissed = 3
}