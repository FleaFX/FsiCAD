using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;

namespace FsiCAD.Components;

/// <summary>
/// Captures the reply when replying to a message dispatched by a <see cref="MessageDispatcher{TMessage}"/>.
/// </summary>
/// <typeparam name="T">The type of the reply message.</typeparam>
/// <param name="reply">The reply message to capture.</param>
public delegate void ReplySink<in T>(T reply);

public static class MessageDispatcher {
    /// <summary>
    /// Gets a <see cref="TimeSpan"/> that you can use to pass to the <see cref="MessageDispatcher{TMessage}.DispatchAndReplyAsync{TResult}"/> method, currently configured to 10 seconds.
    /// </summary>
    public static readonly TimeSpan ReplyTimeout = TimeSpan.FromSeconds(10);
}

public class MessageDispatcher<TMessage> : IObservable<TMessage> {
    readonly ILogger<MessageDispatcher<TMessage>>? _logger;
    readonly Subject<TMessage> _subject = new();

    /// <summary>
    /// Initializes a new <see cref="MessageDispatcher{TMessage}"/>.
    /// </summary>
    /// <param name="logger">Optional. A <see cref="ILogger"/> to use.</param>
    public MessageDispatcher(ILogger<MessageDispatcher<TMessage>>? logger = null) =>
        _logger = logger;

    /// <summary>
    /// Dispatches the given <paramref name="message"/> to the subscribers.
    /// </summary>
    /// <param name="message">The <typeparamref name="TMessage"/> to dispatch.</param>
    public virtual void Dispatch(TMessage message) {
        if (!_subject.HasObservers)
            _logger?.LogWarning($"A {typeof(TMessage).Name} was dispatched but there are no subscribers for this message type.");

        _subject.OnNext(message);
    }

    /// <summary>
    /// Asynchronously dispatches the given <paramref name="message"/> to the subscribers.
    /// </summary>
    /// <param name="message">The message to dispatch.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
    /// <returns></returns>
    public virtual ValueTask DispatchAsync(TMessage message, CancellationToken cancellationToken = default) {
        if (!_subject.HasObservers)
            _logger?.LogWarning($"A {typeof(TMessage).Name} was dispatched but there are no subscribers for this message type.");

        if (!cancellationToken.IsCancellationRequested)
            _subject.OnNext(message);

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Asynchronously dispatches a message and replies with a <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the message to reply.</typeparam>
    /// <param name="messageFactory">
    /// A <see cref="Func{TResult}"/> that takes a <see cref="ReplySink{T}"/> produces the message to dispatch.
    /// Use the <see cref="ReplySink{T}"/> as a callback to produce the reply to return from this method.
    /// The method will not return until this callback is invoked.
    /// </param>
    /// <param name="replyTimeout">Optional. A <see cref="TimeSpan"/> after which the reply should timeout. If omitted, the dispatcher will indefinitely wait for a reply to come.</param>
    /// <param name="defaultOnTimeout">Optional. A <typeparamref name="TResult"/> to return if the reply times out. If omitted, a <see cref="TaskCanceledException"/> will be thrown on timeout.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the asynchronous operation.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> that represents the asynchronous operation.</returns>
    public virtual async ValueTask<TResult> DispatchAndReplyAsync<TResult>(Func<ReplySink<TResult>, TMessage> messageFactory, TimeSpan? replyTimeout = null, TResult? defaultOnTimeout = default, CancellationToken cancellationToken = default) {
        var taskCompletionSource = new TaskCompletionSource<TResult>();
        var timeout = Task.Delay((int)(replyTimeout?.TotalMilliseconds ?? -1), cancellationToken);
        await DispatchAsync(messageFactory(taskCompletionSource.SetResult), cancellationToken);
        var winner = await Task.WhenAny(taskCompletionSource.Task, timeout);
        if (!winner.Equals(timeout))
            return await taskCompletionSource.Task;

        taskCompletionSource.SetCanceled(cancellationToken);

        return defaultOnTimeout ?? throw new TaskCanceledException(taskCompletionSource.Task);
    }

    /// <summary>Notifies the provider that an observer is to receive notifications.</summary>
    /// <param name="observer">The object that is to receive notifications.</param>
    /// <returns>A reference to an interface that allows observers to stop receiving notifications before the provider has finished sending them.</returns>
    public virtual IDisposable Subscribe(IObserver<TMessage> observer) =>
        _subject.Subscribe(observer);
}
