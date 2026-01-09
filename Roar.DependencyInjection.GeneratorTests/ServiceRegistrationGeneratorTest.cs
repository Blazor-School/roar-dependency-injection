using Roar.DependencyInjection.Generators;

namespace Roar.DependencyInjection.GeneratorTests;

public class ServiceRegistrationGeneratorTest
{
    [Fact(DisplayName = "When there is no interface and scoped service, it should register without interface")]
    public async Task NotGeneric_GenerateExpectedOutput()
    {
        const string source = """
            using Roar.DependencyInjection.Abstractions;

            namespace ConsumingProject.Services;

            public class MyScopedService : IScopedService
            {
            }
            """;
        const string expectedGenerated = """
            using Microsoft.Extensions.DependencyInjection;

            namespace Roar.DependencyInjection.Generated;

            public static partial class RoarGeneratedModule
            {
                public static IServiceCollection AddRoarServices(this IServiceCollection services)
                {
                    services.AddScoped<ConsumingProject.Services.MyScopedService>();

                    return services;
                }
            }

            """;

        await GeneratorTestHelper.VerifyAsync<DepedencyRegistrationGenerator>(source, expectedGenerated, "RoarGeneratedModule.g.cs");
    }
}
