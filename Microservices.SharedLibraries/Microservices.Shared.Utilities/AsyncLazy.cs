using System.Runtime.CompilerServices;

namespace Microservices.Shared.Utilities;

/// <summary>
/// Provides asynchronous Lazy initialization.
/// </summary>
/// <typeparam name="T">The type being initialized.</typeparam>
public class AsyncLazy<T> : Lazy<Task<T>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{T}"/> class.
    /// </summary>
    /// <param name="valueFactory">The <see cref="Func{T}"/> invoked to produce the lazily-initialized value when it is needed.</param>
    public AsyncLazy(Func<T> valueFactory) : base(() => Task.Factory.StartNew(valueFactory)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncLazy{T}"/> class.
    /// </summary>
    /// <param name="taskFactory">The <see cref="Func{T}"/> invoked to produce the lazily-initialized value when it is needed.</param>
    public AsyncLazy(Func<Task<T>> taskFactory) : base(() => Task.Factory.StartNew(() => taskFactory()).Unwrap()) { }

    /// <summary>
    /// Allows
    ///     var value = await asyncLazyInstance
    /// instead of
    ///     var value = await asyncLazyInstance.Value.
    /// </summary>
    /// <returns>The initialized instance.</returns>
    public TaskAwaiter<T> GetAwaiter() => Value.GetAwaiter();
}
