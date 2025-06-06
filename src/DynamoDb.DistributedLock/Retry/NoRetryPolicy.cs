using System;
using System.Threading;
using System.Threading.Tasks;

namespace DynamoDb.DistributedLock.Retry;

/// <summary>
/// A no-op retry policy that executes operations once without retrying.
/// </summary>
public sealed class NoRetryPolicy : IRetryPolicy
{
    /// <summary>
    /// Gets the singleton instance of the no-retry policy.
    /// </summary>
    public static readonly NoRetryPolicy Instance = new();

    private NoRetryPolicy() { }

    /// <inheritdoc />
    public async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        Func<Exception, bool> shouldRetry,
        CancellationToken cancellationToken = default)
    {
        if (operation == null) throw new ArgumentNullException(nameof(operation));
        
        return await operation(cancellationToken);
    }
}