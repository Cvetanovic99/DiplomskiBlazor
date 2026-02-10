namespace Diplomski.RatingHub.Domain.Interfaces;

public interface IAuditableEntityBase
{
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
}