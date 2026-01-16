using Roar.DependencyInjection.Generators;

namespace Roar.DependencyInjection.Test;

public class GrpcRegistationGeneratorTest
{
    [Fact(DisplayName = "When there is grpc service, it calls MapGrpcService")]
    public async Task Test()
    {
        string consumingNamespace = "ConsumingProject.GrpcServices";

        string source = $$"""
using Roar.DependencyInjection.Abstractions;

namespace {{consumingNamespace}};

public class MyGrpcService : IGrpcService
{
}
""";

        string expectedGenerated = $$"""
using Microsoft.AspNetCore.Builder;

namespace {{consumingNamespace}};

[global::System.CodeDom.Compiler.GeneratedCode("RoarEngine", "1.0.0")]
[global::System.Diagnostics.DebuggerNonUserCode]
[global::System.Diagnostics.DebuggerStepThrough]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
[global::System.Runtime.CompilerServices.CompilerGenerated]
public static class RoarGeneratedModuleGrpc
{
    public static WebApplication MapRoarEndpoints(this WebApplication app)
    {
        app.MapGrpcService<{{consumingNamespace}}.MyGrpcService>();

        return app;
    }
}

""";

        await GeneratorTestHelper.VerifyAsync<DependencyRegistrationGrpcGenerator>(source, expectedGenerated, "RoarGeneratedModuleGrpc.g.cs", consumingNamespace);
    }
}
