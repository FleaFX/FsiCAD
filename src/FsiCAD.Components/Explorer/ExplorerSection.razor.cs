using Microsoft.AspNetCore.Components;

namespace FsiCAD.Components.Explorer;

/// <summary>
/// Base class for explorer sections.
/// </summary>
public partial class ExplorerSection : IDispatchable {
    [Inject] MessageDispatcher<ToggleSection> ToggleSectionDispatcher { get; set; } = null!;
    [CascadingParameter] Explorer Explorer { get; set; } = null!;

    /// <summary>
    /// Indicates whether the section is expanded.
    /// </summary>
    public bool IsExpanded { get; set; }

    protected override async Task OnInitializedAsync() =>
        await Explorer.AddSectionAsync(this);

    public async void OnClickHeader() =>
        await ToggleSectionDispatcher.DispatchAsync(new ToggleSection(this));
}