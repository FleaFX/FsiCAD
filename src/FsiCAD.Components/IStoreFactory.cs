using System.Reactive.Concurrency;

namespace FsiCAD.Components;

public interface IStoreFactory<out TModel> {
    /// <summary>
    /// Creates a <see cref="IObservable{T}"/> that produces instances of <typeparamref name="TModel"/>.
    /// </summary>
    /// <returns>An <see cref="IObservable{T}"/>.</returns>
    IObservable<TModel> CreateStore(IScheduler? scheduler = default);
}
