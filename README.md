# Roar.DependencyInjection

> **Compile-time dependency injection made simple.**

Roar.DependencyInjection removes the need for repetitive service registration code by generating it at compile time. Instead of maintaining long lists of `AddScoped`, `AddSingleton`, `AddTransient`, `AddHostedService`, and `MapGrpcService` calls, you declare intent through attributes—and Roar wires everything automatically.

No reflection. No runtime scanning. Just clean, deterministic, generated code.

---

## Features

- Automatic convention-based service registration  
- Compile-time source generation (AOT and trimming safe)  
- Clear, attribute-driven service roles  
- Deterministic and predictable wiring  
- Zero-reflection runtime behavior  
- Seamless integration with ASP.NET Core and gRPC  

---

## Installation

```bash
dotnet add package Roar.DependencyInjection
```

---

## Basic usage

### Mark services by role

Simply annotate your classes with a lifetime attribute:

```csharp
using Roar.DependencyInjection;

[ScopedService]
public class OrderService
{
}
```

Roar automatically generates:

```csharp
builder.Services.AddScoped<OrderService>();
```

---

### Explicit interface mapping

Want to register a service under a specific interface? Just declare it:

```csharp
[ScopedService]
[AsService(typeof(IOrderService))]
public class OrderService : IOrderService
{
}
```

Generated result:

```csharp
builder.Services.AddScoped<IOrderService, OrderService>();
```

---

### Mapping to multiple interfaces

Roar supports multiple service contracts out of the box:

```csharp
[ScopedService]
[AsService(typeof(IFoo))]
[AsService(typeof(IBar))]
public class MyService : IFoo, IBar
{
}
```

Generated:

```csharp
builder.Services.AddScoped<IFoo, MyService>();
builder.Services.AddScoped<IBar, MyService>();
```

All mappings are validated at compile time to ensure correctness.

---

### Register all services

Once your services are annotated, a single call wires everything:

```csharp
builder.Services.AddRoarServices();
```

No more manual registrations.

---

## gRPC endpoints

Expose gRPC services using the same simple approach:

```csharp
[GrpcService]
public class OrderGrpcService : OrderServiceContract.OrderServiceContractBase
{
}
```

Roar generates:

```csharp
app.MapGrpcService<OrderGrpcService>();
```

### Map all endpoints

```csharp
app.MapRoarEndpoints();
```

All discovered gRPC services are mapped automatically.

---

## Service roles

| Attribute | Purpose |
|--------------------|--------------------------------|
| `ScopedService` | Register as Scoped service |
| `SingletonService` | Register as Singleton service |
| `TransientService` | Register as Transient service |
| `BackgroundWorker` | Register as hosted background service |
| `GrpcService` | Register as gRPC endpoint |

---

## Why Roar?

- Less boilerplate  
- Fewer DI mistakes  
- Cleaner Program.cs  
- Compile-time validation  
- Safe for AOT, trimming, and native compilation  

Roar lets you focus on architecture instead of wiring.

---

## License

MIT
