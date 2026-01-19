using Roar.DependencyInjection;

namespace ServiceIntegrationTest.Services;

[TransientService]
[AsService(typeof(ITransientWithInterface))]
public class TransientWithInterface : ITransientWithInterface
{
}

public interface ITransientWithInterface
{
}