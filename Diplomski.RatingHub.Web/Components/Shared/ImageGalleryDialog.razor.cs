using Microsoft.AspNetCore.Components;

namespace Diplomski.RatingHub.Web.Components.Shared;

public partial class ImageGalleryDialog
{
    [Parameter] public List<string> Images { get; set; } = null!;
    [Parameter] public int StartIndex { get; set; }

    private int CurrentIndex;

    protected override void OnInitialized()
    {
        CurrentIndex = StartIndex < Images.Count ? StartIndex : 0;
    }

    private void Next()
    {
        CurrentIndex = (CurrentIndex + 1) % Images.Count;
    }

    private void Prev()
    {
        CurrentIndex = (CurrentIndex - 1 + Images.Count) % Images.Count;
    }
}