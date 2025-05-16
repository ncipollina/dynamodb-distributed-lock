using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit3;

namespace DynamoDb.DistributedLock.Tests.TestKit.Attributes;

/// <summary>
/// Provides AutoFixture-based data with NSubstitute for use with xUnit theories.
/// </summary>
public class BaseAutoDataAttribute() : AutoDataAttribute(() =>
{
    var fixture = new Fixture();
    fixture.Customize(new AutoNSubstituteCustomization());
    return fixture;
});

/// <summary>
/// Provides inline arguments combined with <see cref="BaseAutoDataAttribute"/> customizations for use with xUnit theories.
/// </summary>
public class InlineBaseAutoDataAttribute(params object[] values)
    : InlineAutoDataAttribute(new BaseAutoDataAttribute(), values);