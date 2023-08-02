using Microsoft.AspNetCore.Components;

namespace FsiCAD.Components;

/// <summary>
/// Base class for components that bind to the provided model.
/// </summary>
/// <typeparam name="TModel">The type of the maintained model.</typeparam>
public abstract class ModelAwareComponentBase<TModel> : ComponentBase, IDisposable {
    IDisposable? _subscription;

    [Inject]
    IStoreFactory<TModel>? StoreFactory { get; set; }

    /// <summary>
    /// Gets the current model.
    /// </summary>
    protected TModel? Model { get; private set; }

    /// <summary>
    /// This method is sealed by the <see cref="ModelAwareComponentBase{TModel}"/> in order not to introduce temporal coupling with regards to the setup of the state change subscription.
    /// If you need to perform initialization logic, override <see cref="OnInitializedCoreAsync"/> instead.
    /// </summary>
    protected sealed override async Task OnInitializedAsync() {
        // create observable
        var store = StoreFactory!.CreateStore();

        // subscribe observer
        _subscription = store.Subscribe(model => {
            Model = model;
            StateHasChanged();
        });

        await OnInitializedCoreAsync();
    }

    /// <summary>
    /// Method invoked when the component is ready to start, having received its initial parameters from its parent in the render tree.
    /// Override this method if you will perform an asynchronous operation and want the component to refresh when that operation is completed.
    /// </summary>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
    protected virtual ValueTask OnInitializedCoreAsync() => ValueTask.CompletedTask;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() =>
        _subscription?.Dispose();
}
