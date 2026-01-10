# Roar.DependencyInjection

> A convention-based dependency injection kernel for .NET.

Roar.DependencyInjection provides convention-driven service composition, compile-time wiring,
and zero-boilerplate dependency registration for .NET applications.

It replaces large blocks of manual `AddScoped`, `AddSingleton`, `AddTransient`, `AddHostedService`, and `MapGrpcService` calls with
architectural conventions and compile-time generated wiring.

This package contains the runtime engine only.  
Source generators, analyzers, and role contracts are delivered by companion packages.

---

## Features

- Convention-based automatic registration  
- Compile-time source generation (AOT and trimming safe)  
- Interface and attribute driven roles  
- Deterministic service composition  
- Zero reflection runtime wiring  
- Web, gRPC, worker, and class library friendly  

---

## Installation

```bash
dotnet add package Roar.DependencyInjection
```

---

## Basic usage

### Mark services by role

```csharp
using Roar.DependencyInjection;

public class OrderService : IScopedService
{
}
```

Explicit interface mapping:

```csharp
public class OrderService : IScopedService<IOrderService>
{
}
```

---

### Register all services

```csharp
builder.Services.AddRoarServices();
```

No manual service wiring is required.

---

## gRPC endpoints

```csharp
public class OrderGrpcService : OrderServiceContract.OrderServiceContractBase, IGrpcService
{
}
```

```csharp
app.MapRoarEndpoints();
```

Roar automatically registers and maps gRPC services.

---

## Lifetime roles

| Role interface | Lifetime |
|---------------|----------|
| `IScopedService` | Scoped |
| `ISingletonService` | Singleton |
| `ITransientService` | Transient |
| `IBackgroundWorker` | Background |
| `IGrpcService` | Grpc Endpoint |

---

## License

MIT
