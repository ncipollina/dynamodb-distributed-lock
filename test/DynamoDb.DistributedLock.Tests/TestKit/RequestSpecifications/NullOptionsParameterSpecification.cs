using System.Reflection;
using AutoFixture.Kernel;
using Microsoft.Extensions.Options;

namespace DynamoDb.DistributedLock.Tests.TestKit.RequestSpecifications;

/// <summary>
/// Matches an IOptions<T> parameter with a specific name to inject a null Value.
/// </summary>
public sealed class NullOptionsParameterSpecification<TOptions> : IRequestSpecification
    where TOptions : class
{
    public bool IsSatisfiedBy(object request)
    {
        return request is ParameterInfo pi &&
               typeof(IOptions<TOptions>).IsAssignableFrom(pi.ParameterType) &&
               string.Equals(pi.Name, "nullOptions", StringComparison.OrdinalIgnoreCase);
    }
}