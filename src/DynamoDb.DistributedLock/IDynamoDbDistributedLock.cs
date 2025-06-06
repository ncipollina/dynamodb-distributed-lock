using System.Threading;
using System.Threading.Tasks;

namespace DynamoDb.DistributedLock;

/// <summary>
/// Defines methods for acquiring and releasing distributed locks using DynamoDB.
/// </summary>
public interface IDynamoDbDistributedLock
{
    /// <summary>
    /// Attempts to acquire a distributed lock on the specified resource.
    /// </summary>
    /// <param name="resourceId">The resource identifier (e.g., a game or operation name).</param>
    /// <param name="ownerId">The unique ID of the lock owner.</param>
    /// <param name="cancellationToken">A cancellation token for the async operation.</param>
    /// <returns><c>true</c> if the lock was successfully acquired; otherwise, <c>false</c>.</returns>
    Task<bool> AcquireLockAsync(string resourceId, string ownerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Releases a previously acquired distributed lock.
    /// </summary>
    /// <param name="resourceId">The resource identifier (e.g., a game or operation name).</param>
    /// <param name="ownerId">The unique ID of the lock owner requesting release.</param>
    /// <param name="cancellationToken">A cancellation token for the async operation.</param>
    /// <returns><c>true</c> if the lock was released; <c>false</c> if the lock was not owned by the caller.</returns>
    Task<bool> ReleaseLockAsync(string resourceId, string ownerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Attempts to acquire a distributed lock on the specified resource and returns a handle for automatic cleanup.
    /// </summary>
    /// <param name="resourceId">The resource identifier (e.g., a game or operation name).</param>
    /// <param name="ownerId">The unique ID of the lock owner.</param>
    /// <param name="cancellationToken">A cancellation token for the async operation.</param>
    /// <returns>An <see cref="IDistributedLockHandle"/> if the lock was successfully acquired; otherwise, <c>null</c>.</returns>
    Task<IDistributedLockHandle?> AcquireLockHandleAsync(string resourceId, string ownerId, CancellationToken cancellationToken = default);
}