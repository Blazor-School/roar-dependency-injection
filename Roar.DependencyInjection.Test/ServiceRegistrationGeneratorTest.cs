using Roar.DependencyInjection.Generators;

namespace Roar.DependencyInjection.Test;

public class ServiceRegistrationGeneratorTest
{
    [Fact(DisplayName = "When there is no interface and scoped service, it should register without interface")]
    public async Task NotGeneric_GenerateExpectedOutput()
    {
        string consumingNamespace = "ConsumingProject.Services";

        string source = $$"""
using Roar.DependencyInjection;

namespace {{consumingNamespace}};

[ScopedService]
public class MyScopedService
{
}
""";
        string expectedGenerated = $$"""
using Microsoft.Extensions.DependencyInjection;

namespace {{consumingNamespace}};

[global::System.CodeDom.Compiler.GeneratedCode("RoarEngine", "2.0.0")]
[global::System.Diagnostics.DebuggerNonUserCode]
[global::System.Diagnostics.DebuggerStepThrough]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
[global::System.Runtime.CompilerServices.CompilerGenerated]
public static class RoarGeneratedModule
{
    public static IServiceCollection AddRoarServices(this IServiceCollection services)
    {
        services.AddScoped<{{consumingNamespace}}.MyScopedService>();

        return services;
    }
}

""";

        await GeneratorTestHelper.VerifyAsync<DepedencyRegistrationGenerator>(source, expectedGenerated, "RoarGeneratedModule.g.cs", consumingNamespace);
    }
}
