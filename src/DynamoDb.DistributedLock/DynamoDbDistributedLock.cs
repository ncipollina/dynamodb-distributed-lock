using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Options;

namespace DynamoDb.DistributedLock;

/// <summary>
/// Implements a DynamoDB-backed distributed lock mechanism.
/// </summary>
public class DynamoDbDistributedLock : IDynamoDbDistributedLock
{
    private readonly IAmazonDynamoDB _client;
    private readonly DynamoDbLockOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbDistributedLock"/> class.
    /// </summary>
    /// <param name="client">The DynamoDB client.</param>
    /// <param name="options">Configuration options for the lock.</param>
    public DynamoDbDistributedLock(IAmazonDynamoDB client,
        IOptions<DynamoDbLockOptions> options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Attempts to acquire a distributed lock on the specified resource.
    /// </summary>
    /// <param name="resourceId">The resource identifier (e.g., a game or operation name).</param>
    /// <param name="ownerId">The unique ID of the lock owner.</param>
    /// <param name="cancellationToken">A cancellation token for the async operation.</param>
    /// <returns><c>true</c> if the lock was acquired; otherwise, <c>false</c>.</returns>
    public async Task<bool> AcquireLockAsync(string resourceId, string ownerId, CancellationToken cancellationToken = default)
    {
        var result = await TryAcquireLockInternalAsync(resourceId, ownerId, cancellationToken);
        return result.IsSuccess;
    }

    /// <summary>
    /// Releases a previously acquired distributed lock.
    /// </summary>
    /// <param name="resourceId">The resource identifier (e.g., a game or operation name).</param>
    /// <param name="ownerId">The unique ID of the lock owner requesting release.</param>
    /// <param name="cancellationToken">A cancellation token for the async operation.</param>
    /// <returns><c>true</c> if the lock was released; <c>false</c> if the lock was not owned by the caller.</returns>
    public async Task<bool> ReleaseLockAsync(string resourceId, string ownerId, CancellationToken cancellationToken = default)
    {
        var request = new DeleteItemRequest
        {
            TableName = _options.TableName,
            Key = new Dictionary<string, AttributeValue>
            {
                [_options.PartitionKeyAttribute] = new() { S = $"lock#{resourceId}" },
                [_options.SortKeyAttribute] = new() { S = "metadata#lock" },
            },
            ConditionExpression = "ownerId = :owner",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":owner"] = new() { S = ownerId }
            }
        };

        try
        {
            await _client.DeleteItemAsync(request, cancellationToken);
            return true; // Lock released
        }
        catch (ConditionalCheckFailedException)
        {
            return false; // Lock was held by another process
        }
    }

    /// <summary>
    /// Attempts to acquire a distributed lock on the specified resource and returns a handle for automatic cleanup.
    /// </summary>
    /// <param name="resourceId">The resource identifier (e.g., a game or operation name).</param>
    /// <param name="ownerId">The unique ID of the lock owner.</param>
    /// <param name="cancellationToken">A cancellation token for the async operation.</param>
    /// <returns>An <see cref="IDistributedLockHandle"/> if the lock was successfully acquired; otherwise, <c>null</c>.</returns>
    public async Task<IDistributedLockHandle?> AcquireLockHandleAsync(string resourceId, string ownerId, CancellationToken cancellationToken = default)
    {
        var result = await TryAcquireLockInternalAsync(resourceId, ownerId, cancellationToken);
        return result.IsSuccess ? new DistributedLockHandle(this, resourceId, ownerId, result.ExpiresAt) : null;
    }

    private async Task<LockAcquisitionResult> TryAcquireLockInternalAsync(string resourceId, string ownerId, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddSeconds(_options.LockTimeoutSeconds);
        var expiresAtUnix = expiresAt.ToUnixTimeSeconds();

        var request = new PutItemRequest
        {
            TableName = _options.TableName,
            Item = new Dictionary<string, AttributeValue>
            {
                [_options.PartitionKeyAttribute] = new() { S = $"lock#{resourceId}" },
                [_options.SortKeyAttribute] = new() { S = "metadata#lock" },
                ["ownerId"] = new() { S = ownerId },
                ["expiresAt"] = new() { N = expiresAtUnix.ToString() }
            },
            ConditionExpression = "attribute_not_exists(pk) AND attribute_not_exists(sk) OR expiresAt < :now",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":now"] = new() { N = now.ToUnixTimeSeconds().ToString() }
            }
        };

        try
        {
            await _client.PutItemAsync(request, cancellationToken);
            return new LockAcquisitionResult(true, expiresAt);
        }
        catch (ConditionalCheckFailedException)
        {
            return new LockAcquisitionResult(false, default);
        }
    }

    private readonly record struct LockAcquisitionResult(bool IsSuccess, DateTimeOffset ExpiresAt);
}