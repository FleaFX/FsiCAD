using Microsoft.AspNetCore.Components;

namespace FsiCAD.Components.ActivityBar; 

public partial class ActivityBar {
    [Inject] MessageDispatcher<Activity> ActivityDispatcher { get; set; } = null!;

    /// <summary>
    /// Asynchronously adds an <see cref="Activity"/> to the bar.
    /// </summary>
    /// <param name="activity">The <see cref="Activity"/> to add.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public ValueTask AddActivityAsync(Activity activity, CancellationToken cancellationToken = default) =>
        ActivityDispatcher.DispatchAsync(activity, cancellationToken);
}