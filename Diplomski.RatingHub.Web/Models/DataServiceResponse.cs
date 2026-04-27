namespace Diplomski.RatingHub.Web.Models;

public class DataServiceResponse<TResult>
{
    public bool ExceptionOccurred { get; set; }
    public TResult? Result { get; set; }
}