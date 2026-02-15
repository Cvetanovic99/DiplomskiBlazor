namespace Diplomski.RatingHub.Application.Models;

public class QueryArgs
{
    public string? Filter { get; set; }
    public string? OrderBy { get; set; }
    public int? Skip { get; set; }
    public int? Take { get; set; }
}