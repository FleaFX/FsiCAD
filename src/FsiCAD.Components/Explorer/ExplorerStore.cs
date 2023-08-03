using System.Reactive.Concurrency;
using System.Reactive.Linq;
using FsiCAD.Core.Extensions.Reactive;

namespace FsiCAD.Components.Explorer;

#region Types
public record struct ExplorerState(IEnumerable<ExplorerSection> Sections) {
    public ExplorerState() : this(Enumerable.Empty<ExplorerSection>()) {}
}

public record struct ToggleSection(ExplorerSection Section) : IDispatchable;
#endregion

public class ExplorerStore : IStoreFactory<ExplorerState> {
    readonly IObservable<ExplorerSection> _explorerSections;
    readonly IObservable<ToggleSection> _sectionToggles;

    public ExplorerStore(
        IObservable<ExplorerSection> explorerSections,
        IObservable<ToggleSection> sectionToggles) {
        _explorerSections = explorerSections;
        _sectionToggles = sectionToggles;
    }

    public IObservable<ExplorerState> CreateStore(IScheduler? scheduler = default) =>
        BuildSections()
            .SwitchMap(ToggleSections);

    /// <summary>
    /// Builds a <see cref="IObservable{T}"/> that produces the list of explorer sections.
    /// </summary>
    IObservable<ExplorerState> BuildSections() =>
        _explorerSections
            .Scan(new ExplorerState(), (state, section) =>
                state with {
                    Sections = state.Sections.Append(section)
                });

    /// <summary>
    /// Builds a <see cref="IObservable{T}"/> that expands or collapses the toggled section.
    /// </summary>
    IObservable<ExplorerState> ToggleSections(ExplorerState state) =>
        _sectionToggles
            .Do(toggle => {
                var newState = !toggle.Section.IsExpanded;

                // collapse all sections first
                foreach (var section in state.Sections) {
                    section.IsExpanded = false;
                }

                toggle.Section.IsExpanded = newState;
            })
            .Select(_ => state);
}