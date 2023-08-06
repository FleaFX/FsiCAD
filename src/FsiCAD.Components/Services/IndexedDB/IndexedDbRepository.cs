using System.Runtime.CompilerServices;

namespace FsiCAD.Components.Services.IndexedDB; 

public class IndexedDbRepository<T> : IRepository<T> {
    readonly IndexedDbConnectionFactory _connectionFactory;

    /// <summary>
    /// Initializes a new <see cref="IndexedDbRepository{T}"/>.
    /// </summary>
    /// <param name="connectionFactory">THe <see cref="IndexedDbConnectionFactory"/> to use when connecting to the IndexedDB.</param>
    public IndexedDbRepository(IndexedDbConnectionFactory connectionFactory) {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Asynchronously retrieves the single entity that satisfies the given <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">Filters the returned result set only returning items that satisfy a given condition.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.</returns>
    public ValueTask<T> GetAsync(Predicate<T>? filter = default, CancellationToken cancellationToken = default) =>
        FindAsync(filter, cancellationToken).SingleAsync(cancellationToken);

    /// <summary>
    /// Asynchronously retrieves all the entities that satisfy the given <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">Filters the returned result set only returning items that satisfy a given condition.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.</returns>
    public async IAsyncEnumerable<T> FindAsync(Predicate<T>? filter = default, [EnumeratorCancellation] CancellationToken cancellationToken = default) {
        await using var db = await _connectionFactory.OpenDatabaseAsync<T>(cancellationToken);

        await foreach (var item in db.FindAsync<T>(cancellationToken)) {
            if (filter?.Invoke(item) ?? true)
                yield return item;
        }
    }
}