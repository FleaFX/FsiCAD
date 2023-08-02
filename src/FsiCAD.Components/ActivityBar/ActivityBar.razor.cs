using FsiCAD.Components.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FsiCAD.Components.ActivityBar; 

public partial class ActivityBar {
    [Inject] IJSRuntime JsRuntime { get; set; }
    [Inject] MessageDispatcher<Activity> ActivityDispatcher { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (Model.ActiveActivity is { }) {
            await using var module = await JsRuntime.ImportModuleAsync("resize");
            await module.InvokeVoidAsync("addResize");
        }
    }

    /// <summary>
    /// Asynchronously adds an <see cref="Activity"/> to the bar.
    /// </summary>
    /// <param name="activity">The <see cref="Activity"/> to add.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public ValueTask AddActivityAsync(Activity activity, CancellationToken cancellationToken = default) =>
        ActivityDispatcher.DispatchAsync(activity, cancellationToken);
}