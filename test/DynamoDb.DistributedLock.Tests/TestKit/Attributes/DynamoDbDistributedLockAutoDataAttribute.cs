using AutoFixture;
using AutoFixture.Xunit3;
using DynamoDb.DistributedLock.Tests.TestKit.Customizations;

namespace DynamoDb.DistributedLock.Tests.TestKit.Attributes;

/// <summary>
/// Provides AutoFixture-based data with NSubstitute and DynamoDb.DistributedLock-specific customizations for use with xUnit theories.
/// </summary>
public class DynamoDbDistributedLockAutoDataAttribute() : AutoDataAttribute(() =>
{
    var fixture = new Fixture();
    fixture.Customize(new LockingCustomization());
    return fixture;
});

/// <summary>
/// Provides inline arguments combined with <see cref="DynamoDbDistributedLockAutoDataAttribute"/> customizations for use with xUnit theories.
/// </summary>
public class InlineDynamoDbDistributedLockAutoDataAttribute(params object[] values)
    : InlineAutoDataAttribute(new DynamoDbDistributedLockAutoDataAttribute(), values);
    