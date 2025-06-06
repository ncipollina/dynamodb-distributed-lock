using System;
using System.Threading.Tasks;

namespace DynamoDb.DistributedLock;

/// <summary>
/// Represents a handle to an acquired distributed lock that supports automatic cleanup via IAsyncDisposable.
/// </summary>
public interface IDistributedLockHandle : IAsyncDisposable
{
    /// <summary>
    /// Gets the resource identifier that this lock is protecting.
    /// </summary>
    string ResourceId { get; }
    
    /// <summary>
    /// Gets the unique identifier of the lock owner.
    /// </summary>
    string OwnerId { get; }
    
    /// <summary>
    /// Gets the UTC timestamp when this lock will expire.
    /// </summary>
    DateTimeOffset ExpiresAt { get; }
    
    /// <summary>
    /// Gets a value indicating whether this lock is currently acquired and valid.
    /// </summary>
    bool IsAcquired { get; }
}