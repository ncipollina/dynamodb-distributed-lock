using System;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DynamoDb.DistributedLock.Extensions;

/// <summary>
/// Extension methods for registering DynamoDB distributed lock services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the DynamoDB distributed lock using configuration bound from the specified section.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="configuration">The configuration source.</param>
    /// <param name="sectionName">The name of the configuration section to bind to <see cref="DynamoDbLockOptions"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddDynamoDbDistributedLock(this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = DynamoDbLockOptions.DynamoDbLockSettings)
    {
        return services.AddDynamoDbDistributedLock(options => configuration.GetSection(sectionName).Bind(options));
    }

    /// <summary>
    /// Registers the DynamoDB distributed lock using a delegate to configure <see cref="DynamoDbLockOptions"/>.
    /// </summary>
    /// <param name="services">The service collection to register with.</param>
    /// <param name="configure">The delegate to configure <see cref="DynamoDbLockOptions"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddDynamoDbDistributedLock(this IServiceCollection services,
        Action<DynamoDbLockOptions> configure)
    {
        services.Configure(configure);
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddSingleton<IDynamoDbDistributedLock, DynamoDbDistributedLock>();
        return services;
    }
}