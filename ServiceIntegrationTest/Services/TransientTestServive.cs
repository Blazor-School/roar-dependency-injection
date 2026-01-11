using Roar.DependencyInjection.Abstractions;

namespace ServiceIntegrationTest.Services;

public class TransientTestServive : ITransientService
{
    public string GetMessage() => "Hello from Transient Service";
}
