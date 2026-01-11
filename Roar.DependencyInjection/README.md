# Roar.DependencyInjection

> Compile-time service registration for .NET.

Replaces large blocks of manual `AddScoped`, `AddSingleton`, `AddTransient`, `AddHostedService`, and `MapGrpcService` calls with architectural conventions and compile time generated wiring.

---

## Features

- Convention-based automatic registration  
- Compile-time source generation (AOT and trimming safe)  
- Interface driven roles  
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

This will generate:

```csharp
builder.Services.AddScoped<OrderService>();
```

Explicit interface mapping:

```csharp
public class OrderService : IScopedService<IOrderService>
{
}
```

This will generate:

```csharp
builder.Services.AddScoped<IOrderService, OrderService>();
```
---

### Register all services

```csharp
builder.Services.AddRoarServices();
```

---

## gRPC endpoints

```csharp
public class OrderGrpcService : OrderServiceContract.OrderServiceContractBase, IGrpcService
{
}
```

This will generate:

```csharp
app.MapGrpcService<OrderGrpcService>();
```

### Register all services
```csharp
app.MapRoarEndpoints();
```

---

## Lifetime roles

| Role interface | Lifetime
|---------------|----------
| `IScopedService` | Scoped 
| `ISingletonService` | Singleton 
| `ITransientService` | Transient 
| `IBackgroundWorker` | Background 
| `IGrpcService` | gRPC endpoint

---

## License

MIT
