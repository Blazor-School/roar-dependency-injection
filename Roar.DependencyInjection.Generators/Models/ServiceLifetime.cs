namespace Roar.DependencyInjection.Generators.Models;

public enum ServiceLifetime
{
    Transient,
    Scoped,
    Singleton,
    Background,
    Grpc
}
