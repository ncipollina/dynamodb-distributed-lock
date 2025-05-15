using AutoFixture.Kernel;
using DynamoDb.DistributedLock.Tests.TestKit.RequestSpecifications;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace DynamoDb.DistributedLock.Tests.TestKit.SpecimenBuilders;

/// <summary>
/// Returns an IOptions&lt;T&gt; where Value is null, when matched by the provided specification.
/// </summary>
/// <param name="spec">The request specification used to determine whether to return a null-valued IOptions instance.</param>
public sealed class NullOptionsSpecimenBuilder<TOptions>(IRequestSpecification spec) : ISpecimenBuilder
    where TOptions : class
{
    public NullOptionsSpecimenBuilder()
        : this(new NullOptionsParameterSpecification<TOptions>())
    {
    }

    private readonly IRequestSpecification _spec = spec;

    public object Create(object request, ISpecimenContext context)
    {
        if (!_spec.IsSatisfiedBy(request))
            return new NoSpecimen();

        var substitute = Substitute.For<IOptions<TOptions>>();
        substitute.Value.Returns((TOptions)null!);
        return substitute;
    }
}