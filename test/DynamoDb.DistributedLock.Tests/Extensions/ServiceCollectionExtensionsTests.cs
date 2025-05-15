using Amazon.DynamoDBv2;
using DynamoDb.DistributedLock.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace DynamoDb.DistributedLock.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDynamoDbDistributedLock_WithAction_SetsUpServiceAndOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDynamoDbDistributedLock(options =>
        {
            options.TableName = "locks";
            options.LockTimeoutSeconds = 45;
        });

        // 👇 Override with mock AFTER to bypass credential resolution
        services.AddSingleton(Substitute.For<IAmazonDynamoDB>());
        
        var provider = services.BuildServiceProvider();

        // Assert
        var lockService = provider.GetService<IDynamoDbDistributedLock>();
        lockService.Should().NotBeNull();

        var options = provider.GetRequiredService<IOptions<DynamoDbLockOptions>>().Value;
        options.TableName.Should().Be("locks");
        options.LockTimeoutSeconds.Should().Be(45);
    }

    [Fact]
    public void AddDynamoDbDistributedLock_WithConfiguration_BindsOptionsCorrectly()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            ["DynamoDbLock:TableName"] = "my-table",
            ["DynamoDbLock:LockTimeoutSeconds"] = "60"
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        var services = new ServiceCollection();

        // Act
        services.AddDynamoDbDistributedLock(configuration);
        // 👇 Override with mock AFTER to bypass credential resolution
        services.AddSingleton(Substitute.For<IAmazonDynamoDB>());
        var provider = services.BuildServiceProvider();

        // Assert
        var lockService = provider.GetService<IDynamoDbDistributedLock>();
        lockService.Should().NotBeNull();

        var options = provider.GetRequiredService<IOptions<DynamoDbLockOptions>>().Value;
        options.TableName.Should().Be("my-table");
        options.LockTimeoutSeconds.Should().Be(60);
    }
}
