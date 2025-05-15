using AutoFixture;
using AutoFixture.Kernel;
using DynamoDb.DistributedLock.Tests.TestKit.RequestSpecifications;
using Microsoft.Extensions.Options;

namespace DynamoDb.DistributedLock.Tests.TestKit.SpecimenBuilders;

/// <summary>
/// Generates a valid <see cref="IOptions{TOptions}"/> instance when the request satisfies the valid options specification.
/// </summary>
public sealed class OptionsSpecimenBuilder<TOptions>(IRequestSpecification spec) : ISpecimenBuilder
    where TOptions : class
{
    public OptionsSpecimenBuilder() : this(new OptionsParameterSpecification<TOptions>())
    {
    }

    private readonly IRequestSpecification _spec = spec;
    
    public object Create(object request, ISpecimenContext context)
    {
        if (!_spec.IsSatisfiedBy(request))
            return new NoSpecimen();

        var value = context.Create<TOptions>();
        return Options.Create(value);
    }
}