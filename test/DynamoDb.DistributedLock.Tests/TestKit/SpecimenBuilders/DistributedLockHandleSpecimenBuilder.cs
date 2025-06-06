using AutoFixture.Kernel;
using System.Reflection;
using AutoFixture;

namespace DynamoDb.DistributedLock.Tests.TestKit.SpecimenBuilders;

/// <summary>
/// Creates instances of <see cref="DistributedLockHandle"/> for testing purposes.
/// </summary>
public class DistributedLockHandleSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof(DistributedLockHandle))
            return new NoSpecimen();

        var lockService = context.Create<IDynamoDbDistributedLock>();
        var resourceId = context.Create<string>();
        var ownerId = context.Create<string>();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(5); // Default to 5 minutes from now

        // Use reflection to create the internal class
        var constructor = type.GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance, 
            null, 
            new[] { typeof(IDynamoDbDistributedLock), typeof(string), typeof(string), typeof(DateTimeOffset) }, 
            null);

        if (constructor == null)
            return new NoSpecimen();

        return constructor.Invoke(new object[] { lockService, resourceId, ownerId, expiresAt });
    }
}