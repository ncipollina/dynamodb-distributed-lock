# 🔒 DynamoDb.DistributedLock

**DynamoDb.DistributedLock** is a lightweight .NET library for distributed locking using Amazon DynamoDB. It is designed for serverless and cloud-native applications that require coordination across services or instances.

- ✅ Safe and atomic lock acquisition using conditional writes
- ✅ TTL-based expiration to prevent stale locks
- ✅ AWS-native, no external infrastructure required
- ✅ Simple `IDynamoDbDistributedLock` interface
- ✅ Tested and production-ready for .NET 8 and 9

---

## 📦 Package

| Package                     | Build | NuGet                                                                                                                                                                                      | Downloads                                                                                 |
|----------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------|
| **DynamoDb.DistributedLock** | [![Build](https://github.com/ncipollina/dynamodb-distributed-lock/actions/workflows/build.yaml/badge.svg)](https://github.com/ncipollina/dynamodb-distributed-lock/actions/workflows/build.yaml) | [![NuGet](https://img.shields.io/nuget/v/DynamoDb.DistributedLock.svg)](https://www.nuget.org/packages/DynamoDb.DistributedLock)                                                           | [![NuGet Downloads](https://img.shields.io/nuget/dt/DynamoDb.DistributedLock.svg)](https://www.nuget.org/packages/DynamoDb.DistributedLock) |

---

## 🚀 Getting Started

### 1. Install the NuGet package

```bash
dotnet add package DynamoDb.DistributedLock
```

### 2. Register the lock in your DI container

```csharp
services.AddDynamoDbDistributedLock(options =>
{
    options.TableName = "my-lock-table";
    options.LockTimeoutSeconds = 30;
    options.PartitionKeyAttribute = "pk";
    options.SortKeyAttribute = "sk";
});
```

Or bind from configuration:

```csharp
services.AddDynamoDbDistributedLock(configuration);
```

### appsettings.json
```json
{
  "DynamoDbLock": {
    "TableName": "my-lock-table",
    "LockTimeoutSeconds": 30,
    "PartitionKeyAttribute": "pk",
    "SortKeyAttribute": "sk"
  }
}
```

### 3. Use the lock

```csharp
public class MyService(IDynamoDbDistributedLock distributedLock)
{
    public async Task<bool> TryDoWorkAsync()
    {
        var acquired = await distributedLock.AcquireLockAsync("resource-1", "owner-abc", CancellationToken.None);
        if (!acquired) return false;

        try
        {
            // 🔧 Critical section
        }
        finally
        {
            await distributedLock.ReleaseLockAsync("resource-1", "owner-abc", CancellationToken.None);
        }

        return true;
    }
}
```

---

## 🏗️ Table Schema

This library supports both dedicated tables and shared, single-table designs. You do not need to create a separate table just for locking — this works seamlessly alongside your existing entities.

By default, the library uses the following attributes:

- Partition key: `pk` (String)
- Sort key: `sk` (String)
- TTL attribute: `expiresAt` (Number, UNIX timestamp in seconds)

However, the partition and sort key attribute names are fully configurable via `DynamoDbLockOptions`. This makes it easy to integrate into your existing table structure.
> ✅ Enable TTL on the expiresAt field in your table settings to allow automatic cleanup of expired locks.

---

## 🧪 Unit Testing

Unit tests are written with:

- ✅ xUnit v3
- ✅ AutoFixture + NSubstitute
- ✅ FluentAssertions (AwesomeAssertions)

The library provides `DynamoDbDistributedLockAutoData` to support streamlined tests with frozen mocks and null-value edge cases.

---

## 🔮 Future Enhancements

- ⚙️ Configurable partition/sort key field names
- ⏱ Lock renewal support
- 🔁 Auto-release logic for expired locks
- 📈 Metrics and diagnostics support

---

## 📜 License

MIT

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## 🤝 Contributing

Contributions, feedback, and GitHub issues welcome!
