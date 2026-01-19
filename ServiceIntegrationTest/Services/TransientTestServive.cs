using Roar.DependencyInjection;

namespace ServiceIntegrationTest.Services;

[TransientService]
public class TransientTestServive
{
    public string GetMessage() => "Hello from Transient Service";
}
