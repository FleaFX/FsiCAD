using System.Reactive.Concurrency;
using System.Reactive.Linq;
using FsiCAD.Core.Extensions.Reactive;

namespace FsiCAD.Components.ActivityBar;

#region Types

public record struct ActivityBarState(IEnumerable<Activity> Activities) {
    public ActivityBarState() : this(Enumerable.Empty<Activity>()) { }
}

public record struct ToggleActivity(Activity Activity) : IDispatchable;
#endregion

public class ActivityBarStore : IStoreFactory<ActivityBarState> {
    readonly IObservable<Activity> _activities;
    readonly IObservable<ToggleActivity> _activityToggles;

    /// <summary>
    /// Initializes a new <see cref="ActivityBarStore"/>.
    /// </summary>
    /// <param name="activities">Each <see cref="Activity"/> produced by this observable is added to the list of activities.</param>
    /// <param name="activityToggles">Produces a <see cref="Activity"/> to toggle when the user clicks an activity button.</param>
    public ActivityBarStore(
        IObservable<Activity> activities,
        IObservable<ToggleActivity> activityToggles
    ) {
        _activities = activities;
        _activityToggles = activityToggles;
    }

    /// <summary>
    /// Creates a <see cref="IObservable{T}"/> that produces instances of <see cref="ActivityBarState"/>.
    /// </summary>
    /// <returns>An <see cref="IObservable{T}"/>.</returns>
    public IObservable<ActivityBarState> CreateStore(IScheduler? scheduler = default) =>
        BuildActivities()
            .SwitchMap(ToggleActivities);

    /// <summary>
    /// Builds a <see cref="IObservable{T}"/> that produces the list of activities.
    /// </summary>
    IObservable<ActivityBarState> BuildActivities() =>
        _activities
            .Scan(new ActivityBarState(), (state, activity) =>
                state with { Activities = state.Activities.Append(activity) }
            );

    /// <summary>
    /// Builds a <see cref="IObservable{T}"/> that marks the active activity in the list.
    /// </summary>
    IObservable<ActivityBarState> ToggleActivities(ActivityBarState state) =>
        _activityToggles
            .Do(toggle => {
                var newState = !toggle.Activity.IsActive;

                // deactivate all activities first
                foreach (var activity in state.Activities) {
                    activity.IsActive = false;
                }

                toggle.Activity.IsActive = newState;
            })
            .Select(_ => state);
}