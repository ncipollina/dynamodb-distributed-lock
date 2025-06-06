using System;

namespace DynamoDb.DistributedLock.Retry;

/// <summary>
/// Configuration options for retry behavior when acquiring distributed locks.
/// </summary>
public sealed class RetryOptions
{
    /// <summary>
    /// Gets or sets the maximum number of retry attempts. Default is 3.
    /// </summary>
    public int MaxAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the base delay between retry attempts. Default is 100ms.
    /// </summary>
    public TimeSpan BaseDelay { get; set; } = TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// Gets or sets the maximum delay between retry attempts. Default is 5 seconds.
    /// </summary>
    public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets or sets the multiplier for exponential backoff. Default is 2.0.
    /// </summary>
    public double BackoffMultiplier { get; set; } = 2.0;

    /// <summary>
    /// Gets or sets whether to add jitter to retry delays to avoid thundering herd. Default is true.
    /// </summary>
    public bool UseJitter { get; set; } = true;

    /// <summary>
    /// Gets or sets the jitter factor as a percentage of the delay (0.0 to 1.0). Default is 0.25 (25%).
    /// </summary>
    public double JitterFactor { get; set; } = 0.25;

    /// <summary>
    /// Gets or sets whether retry is enabled. Default is false to maintain backward compatibility.
    /// </summary>
    public bool Enabled { get; set; } = false;
}