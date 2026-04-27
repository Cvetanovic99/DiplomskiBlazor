using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Diplomski.RatingHub.Application.Exceptions;
using Diplomski.RatingHub.Web.Models;

namespace Diplomski.RatingHub.Web.Components.Shared;

public class PortalComponentBase : ComponentBase
{
    [Inject] protected IJSRuntime JSRuntime { get; set; }
    [Inject] protected DialogService DialogService { get; set; }
    [Inject] protected NotificationService NotificationService { get; set; }
    [Inject] protected TooltipService TooltipService { get; set; }
    
    protected bool IsLoading { get; set; }
    
    private const string _tooltipBackgroundColor = "#616161";
    private const int _tooltipFontSizeInPixels = 13;
    private const int _tooltipDurationInMilliseconds = 4000;
    
    
    protected async Task<DataServiceResponse<TResult>> InvokeDataServiceMethod<TResult>(Func<Task<TResult>> method,
            string? successMessage = null, string? errorMessage = null, bool invokeStateHasChanged = false)
        {
            var dataServiceResponse = new DataServiceResponse<TResult>();
            if (string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "Doslo je do greske: ";
            }

            try
            {
                IsLoading = true;
                await Task.Yield();

                if (invokeStateHasChanged)
                {
                    await InvokeAsync(StateHasChanged);
                }
                
                dataServiceResponse.Result = await method();
            }
            catch (ValidationException validationException)
            {
                errorMessage += validationException.Failures.Aggregate(string.Empty,
                    (current, failure) => current + $"{failure.Value[0]} \n");
                dataServiceResponse.ExceptionOccurred = true;
            }
            catch (Exception exception)
            {
                errorMessage += exception.Message;
                dataServiceResponse.ExceptionOccurred = true;
            }
            finally
            {
                IsLoading = false;
                await InvokeAsync(StateHasChanged);
            }

            if (dataServiceResponse.ExceptionOccurred)
            {
                ShowNotification(errorMessage, NotificationSeverity.Error);
            }

            if (!dataServiceResponse.ExceptionOccurred && !string.IsNullOrEmpty(successMessage))
            {
                ShowNotification(successMessage, NotificationSeverity.Success);
            }

            return dataServiceResponse;
        }
    
    // Used for exception handling of data services async methods without return type
    protected async Task<bool> InvokeDataServiceMethod(Func<Task> method,
        string? successMessage = null, string? errorMessage = null, bool invokeStateHasChanged = false)
    {
        if (string.IsNullOrEmpty(errorMessage))
        {
            errorMessage = "Doslo je do greske: ";
        }

        var exceptionOccurred = false;
        try
        {
            IsLoading = true;
            await Task.Yield();
                
            if (invokeStateHasChanged)
            {
                await InvokeAsync(StateHasChanged);
            }

                
            await method();
        }
        catch (ValidationException validationException)
        {
            errorMessage += validationException.Failures.Aggregate(string.Empty,
                (current, failure) => current + $"{failure.Value[0]} \n");
            exceptionOccurred = true;
        }
        catch (Exception exception)
        {
            errorMessage += exception.Message;
            exceptionOccurred = true;
        }
        finally
        {
            IsLoading = false;
        }

        if (exceptionOccurred)
        {
            ShowNotification(errorMessage, NotificationSeverity.Error);
        }

        if (!exceptionOccurred && !string.IsNullOrEmpty(successMessage))
        {
            ShowNotification(successMessage, NotificationSeverity.Success);
        }

        return !exceptionOccurred;
    }
    
    protected void ShowNotification(string detail, NotificationSeverity severity = NotificationSeverity.Info,
        string? summary = null, int durationInMilliseconds = 4000)
    {
        var message = new NotificationMessage
        {
            //Style = "position: fixed;z-index: 1002;float: right;right: 10px;top: 80%;",
            Severity = severity,
            Summary = !string.IsNullOrEmpty(summary) ? summary : severity.ToString(),
            Detail = detail,
            Duration = durationInMilliseconds
        };

        NotificationService.Notify(message);
    }
    
    protected void ShowTooltip(ElementReference elementReference, string text,
        TooltipPosition position = TooltipPosition.Bottom,
        int? durationInMilliSeconds = null, string style = null, string cssClass = null)
    {
        var options = new TooltipOptions
        {
            Style = string.IsNullOrEmpty(style)
                ? $"background-color: {_tooltipBackgroundColor}; font-size: {_tooltipFontSizeInPixels}px; color: white;"
                : style,
            Position = position,
            CssClass = cssClass,
            Duration = durationInMilliSeconds ?? _tooltipDurationInMilliseconds,
            CloseTooltipOnDocumentClick = true
        };

        TooltipService.Open(elementReference, text, options);
    }
}