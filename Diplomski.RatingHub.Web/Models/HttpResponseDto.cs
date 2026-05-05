namespace Diplomski.RatingHub.Web.Models;

public class HttpResponseDto<TResult>
{
    public bool ExceptionOccurred { get; set; }
    public TResult? Result { get; set; }
}