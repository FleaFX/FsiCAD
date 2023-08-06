using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using FsiCAD.Components.Services;
using Microsoft.JSInterop;

namespace FsiCAD.Components.Explorer.Projects;

#region Types

public record struct Project(string Name);

public record struct ProjectsSectionState(IEnumerable<Project> Projects) {
    public ProjectsSectionState() : this(Array.Empty<Project>()) { }
}
#endregion

public class ProjectsSectionStore : IStoreFactory<ProjectsSectionState> {
    readonly IRepository<Project> _projectRepository;

    public ProjectsSectionStore(IRepository<Project> projectRepository) {
        _projectRepository = projectRepository;
    }

    /// <summary>
    /// Creates a <see cref="IObservable{T}"/> that produces instances of <see cref="ProjectsS"/>
    /// </summary>
    /// <returns>An <see cref="IObservable{T}"/>.</returns>
    public IObservable<ProjectsSectionState> CreateStore(IScheduler? scheduler = default) =>
        BuildInitialState();

    IObservable<ProjectsSectionState> BuildInitialState() => Observable
            .FromAsync<IEnumerable<Project>>(async cancellationToken =>
                await _projectRepository
                    .FindAsync(cancellationToken: cancellationToken)
                    .ToArrayAsync(cancellationToken)
            )
            .Catch<IEnumerable<Project> , Exception >(_ => Observable.Return(Enumerable.Empty<Project>()))
            .Select(projects => new ProjectsSectionState(projects));
}