using Microsoft.AspNetCore.Components;

namespace FsiCAD.Components.ActivityBar; 

public partial class Activity : IDispatchable {
    [Inject] MessageDispatcher<ToggleActivity> ToggleActivityDispatcher { get; set; } = null!;

    protected override async Task OnInitializedAsync() =>
        await ActivityBar.AddActivityAsync(this);

    /// <summary>
    /// Asynchronously opens or closes the given <see cref="Activity"/> in the activity bar.
    /// </summary>
    /// <param name="activity">The <see cref="Activity"/> to toggle.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public ValueTask ToggleActivityAsync(Activity activity, CancellationToken cancellationToken = default) =>
        ToggleActivityDispatcher.DispatchAsync(new ToggleActivity(activity), cancellationToken);
}