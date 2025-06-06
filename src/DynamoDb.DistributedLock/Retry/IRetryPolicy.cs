using System;
using System.Threading;
using System.Threading.Tasks;

namespace DynamoDb.DistributedLock.Retry;

/// <summary>
/// Defines a retry policy for lock acquisition operations.
/// </summary>
public interface IRetryPolicy
{
    /// <summary>
    /// Executes an operation with retry logic.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="shouldRetry">A function that determines if a retry should be attempted based on the exception.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    /// <returns>The result of the operation.</returns>
    Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        Func<Exception, bool> shouldRetry,
        CancellationToken cancellationToken = default);
}