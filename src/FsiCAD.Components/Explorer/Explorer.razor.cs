using Microsoft.AspNetCore.Components;

namespace FsiCAD.Components.Explorer; 

public partial class Explorer {
    [Inject] MessageDispatcher<ExplorerSection> ExplorerSectionDispatcher { get; set; } = null!;

    /// <summary>
    /// Asynchronously adds an <see cref="IExplorerSection"/> to the explorer.
    /// </summary>
    /// <param name="section">The <see cref="IExplorerSection"/> to add.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public ValueTask AddSectionAsync(ExplorerSection section, CancellationToken cancellationToken = default) =>
        ExplorerSectionDispatcher.DispatchAsync(section, cancellationToken);
}