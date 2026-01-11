using Roar.DependencyInjection.Generators;

namespace Roar.DependencyInjection.GeneratorTests;

public class ServiceRegistrationGeneratorTest
{
    [Fact(DisplayName = "When there is no interface and scoped service, it should register without interface")]
    public async Task NotGeneric_GenerateExpectedOutput()
    {
        string consumingNamespace = "ConsumingProject.GrpcServices";

        string source = $@"using Roar.DependencyInjection.Abstractions;

namespace {consumingNamespace};

public class MyScopedService : IScopedService
{{
}}
            ";
        string expectedGenerated = $@"using Microsoft.Extensions.DependencyInjection;

namespace {consumingNamespace};

public static partial class RoarGeneratedModule
{{
    public static IServiceCollection AddRoarServices(this IServiceCollection services)
    {{
        services.AddScoped<{consumingNamespace}.MyScopedService>();

        return services;
    }}
}}
";

        await GeneratorTestHelper.VerifyAsync<DepedencyRegistrationGenerator>(source, expectedGenerated, "RoarGeneratedModule.g.cs", consumingNamespace);
    }
}
