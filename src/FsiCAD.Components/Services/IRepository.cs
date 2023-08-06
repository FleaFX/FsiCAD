namespace FsiCAD.Components.Services;

public interface IRepository<T> {
    /// <summary>
    /// Asynchronously retrieves the single entity that satisfies the given <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">Filters the returned result set only returning items that satisfy a given condition.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.</returns>
    ValueTask<T> GetAsync(Predicate<T>? filter = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves all the entities that satisfy the given <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">Filters the returned result set only returning items that satisfy a given condition.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.</returns>
    IAsyncEnumerable<T> FindAsync(Predicate<T>? filter = default, CancellationToken cancellationToken = default);
}