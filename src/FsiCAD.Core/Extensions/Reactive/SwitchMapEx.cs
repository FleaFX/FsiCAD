using System.Reactive.Linq;

namespace FsiCAD.Core.Extensions.Reactive; 

public static class SwitchMapEx {
    /// <summary>
    /// Projects each source value to an Observable which is merged in the output Observable, emitting values only from the most recently projected Observable
    /// </summary>
    /// <typeparam name="T">The type of the element contained in the source observable.</typeparam>
    /// <typeparam name="TU">The type of the element contained in the target observable.</typeparam>
    /// <param name="source">The source observable.</param>
    /// <param name="project">A function that, when applied to an item emitted by the source Observable, returns an Observable.</param>
    /// <returns>A <see cref="IObservable{T}"/>.</returns>
    public static IObservable<TU> SwitchMap<T, TU>(this IObservable<T> source, Func<T, IObservable<TU>> project) =>
        source.Select(project).Switch();
}