using Diplomski.RatingHub.Domain.Interfaces;

namespace Diplomski.RatingHub.Domain.Models;

public abstract class EntityBase : IDatabaseEntity, IAuditableEntityBase
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Modified { get; set; }
}