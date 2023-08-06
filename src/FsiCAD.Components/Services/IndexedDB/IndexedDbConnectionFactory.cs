using System.Runtime.CompilerServices;
using FsiCAD.Components.Extensions;
using Microsoft.JSInterop;

namespace FsiCAD.Components.Services.IndexedDB; 

public sealed class IndexedDbConnectionFactory {
    readonly IJSRuntime _jsRuntime;
    readonly string _databaseName;
    readonly int _version;

    public IndexedDbConnectionFactory(IJSRuntime jsRuntime, string databaseName, int version = 1) {
        _jsRuntime = jsRuntime;
        _databaseName = databaseName;
        _version = version;
    }

    public async ValueTask<IndexedDbDatabase> OpenDatabaseAsync<T>(CancellationToken cancellationToken = default) {
        await using var module = await _jsRuntime.ImportModuleAsync("indexeddb");

        return new IndexedDbDatabase(
            await module.InvokeAsync<IJSObjectReference>("open", cancellationToken, _databaseName, _version, $"{typeof(T).Name}s")
        );
    }
}

public sealed class IndexedDbDatabase : IAsyncDisposable {
    readonly IJSObjectReference _databaseRef;

    public IndexedDbDatabase(IJSObjectReference databaseRef) {
        _databaseRef = databaseRef;
    }

    public async IAsyncEnumerable<T> FindAsync<T>(CancellationToken cancellationToken = default) {
        var cursor = new CursorCallback<T>();
        await _databaseRef.InvokeVoidAsync("openCursor", cancellationToken, $"{typeof(T).Name}s", DotNetObjectReference.Create(cursor));

        while (!cursor.IsExhausted && !cancellationToken.IsCancellationRequested) {
            yield return await cursor.NextAsync();
        }
    }

    class CursorCallback<T> {
        readonly Queue<T> _values = new();
        readonly object _nextLock = new();
        bool _isExhausted;

        TaskCompletionSource _nextCompletionSource = new();

        /// <summary>
        /// <see langword="true" /> when the cursor has exhausted its values, otherwise <see langword="false" />
        /// </summary>
        public bool IsExhausted => !_values.Any() && _isExhausted;

        /// <summary>
        /// Retrieves the next value from the cursor.
        /// </summary>
        /// <returns></returns>
        public async Task<T> NextAsync() {
            if (!_values.Any() && !_isExhausted) {
                await _nextCompletionSource.Task;
                lock (_nextLock) {
                    _nextCompletionSource = new TaskCompletionSource();
                }
            }

            return _values.Dequeue();
        }

        [JSInvokable]
        public ValueTask YieldAsync(T value) {
            lock (_nextLock) {
                _values.Enqueue(value);
                _nextCompletionSource.SetResult();
            }
            return ValueTask.CompletedTask;
        }

        [JSInvokable]
        public ValueTask BreakAsync() {
            _isExhausted = true;
            _nextCompletionSource.SetCanceled();
            return ValueTask.CompletedTask;
        }
    }

    public ValueTask DisposeAsync() =>
        _databaseRef.DisposeAsync();
}