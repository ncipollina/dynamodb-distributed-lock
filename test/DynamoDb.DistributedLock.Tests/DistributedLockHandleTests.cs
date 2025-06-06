using AutoFixture.Xunit3;
using DynamoDb.DistributedLock.Tests.TestKit.Attributes;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DynamoDb.DistributedLock.Tests;

public class DistributedLockHandleTests
{
    [Theory]
    [DynamoDbDistributedLockAutoData]
    public void Constructor_WhenLockServiceIsNull_ShouldThrowArgumentNullException(
        string resourceId,
        string ownerId,
        DateTimeOffset expiresAt)
    {
        var act = () => new DistributedLockHandle(null!, resourceId, ownerId, expiresAt);

        act.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("lockService");
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public void Constructor_WhenResourceIdIsNull_ShouldThrowArgumentNullException(
        IDynamoDbDistributedLock lockService,
        string ownerId,
        DateTimeOffset expiresAt)
    {
        var act = () => new DistributedLockHandle(lockService, null!, ownerId, expiresAt);

        act.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("resourceId");
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public void Constructor_WhenOwnerIdIsNull_ShouldThrowArgumentNullException(
        IDynamoDbDistributedLock lockService,
        string resourceId,
        DateTimeOffset expiresAt)
    {
        var act = () => new DistributedLockHandle(lockService, resourceId, null!, expiresAt);

        act.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("ownerId");
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public void Properties_ShouldReturnConstructorValues(
        IDynamoDbDistributedLock lockService,
        string resourceId,
        string ownerId,
        DateTimeOffset expiresAt)
    {
        var handle = new DistributedLockHandle(lockService, resourceId, ownerId, expiresAt);

        handle.ResourceId.Should().Be(resourceId);
        handle.OwnerId.Should().Be(ownerId);
        handle.ExpiresAt.Should().Be(expiresAt);
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public void IsAcquired_WhenNotDisposedAndNotExpired_ShouldReturnTrue(
        IDynamoDbDistributedLock lockService,
        string resourceId,
        string ownerId)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(5);
        var handle = new DistributedLockHandle(lockService, resourceId, ownerId, expiresAt);

        handle.IsAcquired.Should().BeTrue();
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public void IsAcquired_WhenExpired_ShouldReturnFalse(
        IDynamoDbDistributedLock lockService,
        string resourceId,
        string ownerId)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(-5);
        var handle = new DistributedLockHandle(lockService, resourceId, ownerId, expiresAt);

        handle.IsAcquired.Should().BeFalse();
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public async Task IsAcquired_WhenDisposed_ShouldReturnFalse(
        IDynamoDbDistributedLock lockService,
        string resourceId,
        string ownerId)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(5);
        var handle = new DistributedLockHandle(lockService, resourceId, ownerId, expiresAt);

        await handle.DisposeAsync();

        handle.IsAcquired.Should().BeFalse();
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public async Task DisposeAsync_ShouldCallReleaseLockAsync(
        [Frozen] IDynamoDbDistributedLock lockService,
        string resourceId,
        string ownerId,
        DateTimeOffset expiresAt)
    {
        var handle = new DistributedLockHandle(lockService, resourceId, ownerId, expiresAt);

        await handle.DisposeAsync();

        await lockService.Received(1).ReleaseLockAsync(resourceId, ownerId, Arg.Any<CancellationToken>());
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public async Task DisposeAsync_WhenCalledMultipleTimes_ShouldOnlyCallReleaseLockOnce(
        [Frozen] IDynamoDbDistributedLock lockService,
        string resourceId,
        string ownerId,
        DateTimeOffset expiresAt)
    {
        var handle = new DistributedLockHandle(lockService, resourceId, ownerId, expiresAt);

        await handle.DisposeAsync();
        await handle.DisposeAsync();
        await handle.DisposeAsync();

        await lockService.Received(1).ReleaseLockAsync(resourceId, ownerId, Arg.Any<CancellationToken>());
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public async Task DisposeAsync_WhenReleaseLockThrows_ShouldSwallowException(
        [Frozen] IDynamoDbDistributedLock lockService,
        string resourceId,
        string ownerId,
        DateTimeOffset expiresAt)
    {
        lockService.ReleaseLockAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Test exception"));

        var handle = new DistributedLockHandle(lockService, resourceId, ownerId, expiresAt);

        var act = async () => await handle.DisposeAsync();

        await act.Should().NotThrowAsync();
    }
}