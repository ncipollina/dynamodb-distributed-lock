using System;
using System.Threading.Tasks;

namespace DynamoDb.DistributedLock;

/// <summary>
/// Represents a handle to an acquired distributed lock that automatically releases the lock when disposed.
/// </summary>
internal sealed class DistributedLockHandle : IDistributedLockHandle
{
    private readonly IDynamoDbDistributedLock _lockService;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DistributedLockHandle"/> class.
    /// </summary>
    /// <param name="lockService">The lock service used to release the lock.</param>
    /// <param name="resourceId">The resource identifier that this lock is protecting.</param>
    /// <param name="ownerId">The unique identifier of the lock owner.</param>
    /// <param name="expiresAt">The UTC timestamp when this lock will expire.</param>
    internal DistributedLockHandle(
        IDynamoDbDistributedLock lockService,
        string resourceId,
        string ownerId,
        DateTimeOffset expiresAt)
    {
        _lockService = lockService ?? throw new ArgumentNullException(nameof(lockService));
        ResourceId = resourceId ?? throw new ArgumentNullException(nameof(resourceId));
        OwnerId = ownerId ?? throw new ArgumentNullException(nameof(ownerId));
        ExpiresAt = expiresAt;
    }

    /// <inheritdoc />
    public string ResourceId { get; }

    /// <inheritdoc />
    public string OwnerId { get; }

    /// <inheritdoc />
    public DateTimeOffset ExpiresAt { get; }

    /// <inheritdoc />
    public bool IsAcquired => !_disposed && DateTimeOffset.UtcNow < ExpiresAt;

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        try
        {
            await _lockService.ReleaseLockAsync(ResourceId, OwnerId);
        }
        catch
        {
            // Intentionally swallow exceptions during disposal to prevent
            // exception propagation that could mask original exceptions
        }
        finally
        {
            _disposed = true;
        }
    }
}