using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AutoFixture.Xunit3;
using DynamoDb.DistributedLock.Tests.TestKit.Attributes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DynamoDb.DistributedLock.Tests;

public class DynamoDbDistributedLockTests
{
    [Theory]
    [DynamoDbDistributedLockAutoData]
    public void Constructor_WhenClientIsNull_ShouldThrowArgumentNullException(IOptions<DynamoDbLockOptions> options)
    {
        Action act = () => _ = new DynamoDbDistributedLock(null!, options);

        act.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("client");
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public void Constructor_WhenOptionsValueIsNull_ShouldThrowArgumentNullException(
        IAmazonDynamoDB client,
        IOptions<DynamoDbLockOptions> nullOptions)
    {
        var act = () => _ = new DynamoDbDistributedLock(client, nullOptions);

        act.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("options");
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public async Task AcquireLockAsync_WhenLockIsAvailable_ShouldReturnTrue(
        [Frozen] IAmazonDynamoDB dynamo,
        DynamoDbDistributedLock sut, string resourceId, string ownerId)
    {
        // Arrange
        dynamo.PutItemAsync(Arg.Any<PutItemRequest>(), Arg.Any<CancellationToken>())
            .Returns(new PutItemResponse());

        // Act
        var result = await sut.AcquireLockAsync(resourceId, ownerId, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public async Task AcquireLockAsync_WhenLockAlreadyExists_ShouldReturnFalse(
        [Frozen] IAmazonDynamoDB dynamo,
        DynamoDbDistributedLock sut, string resourceId, string ownerId)
    {
        // Arrange
        dynamo.PutItemAsync(Arg.Any<PutItemRequest>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new ConditionalCheckFailedException("lock exists"));

        // Act
        var result = await sut.AcquireLockAsync(resourceId, ownerId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public async Task AcquireLockAsync_WhenUnexpectedExceptionOccurs_ShouldThrow(
        [Frozen] IAmazonDynamoDB dynamo,
        DynamoDbDistributedLock sut, string resourceId, string ownerId)
    {
        // Arrange
        dynamo.PutItemAsync(Arg.Any<PutItemRequest>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("unexpected failure"));

        // Act
        var act = async () => await sut.AcquireLockAsync(resourceId, ownerId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public async Task ReleaseLockAsync_WhenOwnerMatches_ShouldReturnTrue(
        [Frozen] IAmazonDynamoDB dynamo,
        DynamoDbDistributedLock sut,
        string resourceId,
        string ownerId)
    {
        // Arrange
        dynamo.DeleteItemAsync(Arg.Any<DeleteItemRequest>(), Arg.Any<CancellationToken>())
            .Returns(new DeleteItemResponse());

        // Act
        var result = await sut.ReleaseLockAsync(resourceId, ownerId, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public async Task ReleaseLockAsync_WhenOwnerDoesNotMatch_ShouldReturnFalse(
        [Frozen] IAmazonDynamoDB dynamo,
        DynamoDbDistributedLock sut,
        string resourceId,
        string ownerId)
    {
        // Arrange
        dynamo.DeleteItemAsync(Arg.Any<DeleteItemRequest>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new ConditionalCheckFailedException("owner mismatch"));

        // Act
        var result = await sut.ReleaseLockAsync(resourceId, ownerId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [DynamoDbDistributedLockAutoData]
    public async Task ReleaseLockAsync_WhenUnexpectedExceptionOccurs_ShouldThrow(
        [Frozen] IAmazonDynamoDB dynamo,
        DynamoDbDistributedLock sut,
        string resourceId,
        string ownerId)
    {
        // Arrange
        dynamo.DeleteItemAsync(Arg.Any<DeleteItemRequest>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("unexpected failure"));

        // Act
        var act = async () => await sut.ReleaseLockAsync(resourceId, ownerId, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}