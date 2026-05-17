namespace Diplomski.RatingHub.Web.Models;

public class ProcessImageResponse
{
    public MultipartFormDataContent? Content { get; set; }
    public bool ExceptionOccured { get; set; }
}