namespace DynamoDb.DistributedLock;

/// <summary>
/// Configuration options for the DynamoDB-based distributed lock.
/// </summary>
public sealed class DynamoDbLockOptions
{
    /// <summary>
    /// The name of the DynamoDB table to use.
    /// </summary>
    public required string TableName { get; init; }

    /// <summary>
    /// Lock timeout duration in seconds.
    /// </summary>
    public int LockTimeoutSeconds { get; init; } = 30;
}