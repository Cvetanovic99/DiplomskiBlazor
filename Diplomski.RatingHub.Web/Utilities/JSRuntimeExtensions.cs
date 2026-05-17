using Microsoft.JSInterop;

namespace Diplomski.RatingHub.Web.Utilities;

public static class JSRuntimeExtensions
{
    public static async Task<bool> SetItemToLocalStorage(this IJSRuntime jsRuntime, string key, string value)
    {
        return await jsRuntime.InvokeAsync<bool>("localStorageHelper.setItem", key, value);
    }
    
    public static async Task<string?> GetItemFromLocalStorage(this IJSRuntime jsRuntime, string key)
    {
        return await jsRuntime.InvokeAsync<string?>("localStorageHelper.getItem", key);
    }
}