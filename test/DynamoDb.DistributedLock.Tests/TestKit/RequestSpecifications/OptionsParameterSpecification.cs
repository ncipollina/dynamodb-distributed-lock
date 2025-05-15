using System.Reflection;
using AutoFixture.Kernel;
using Microsoft.Extensions.Options;

namespace DynamoDb.DistributedLock.Tests.TestKit.RequestSpecifications;

/// <summary>
/// Matches all requests for <see cref="IOptions{T}"/> except when the parameter is named "nullOptions".
/// </summary>
public sealed class OptionsParameterSpecification<TOptions> : IRequestSpecification
    where TOptions : class
{
    public bool IsSatisfiedBy(object request)
    {
        // If someone is asking for IOptions<T> directly
        if (request is Type typeRequest && typeRequest == typeof(IOptions<TOptions>))
            return true;

        // If the request is for a parameter
        if (request is ParameterInfo pi &&
            typeof(IOptions<TOptions>).IsAssignableFrom(pi.ParameterType) &&
            !string.Equals(pi.Name, "nullOptions", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}