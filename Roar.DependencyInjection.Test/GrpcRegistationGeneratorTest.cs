using Roar.DependencyInjection.Generators;

namespace Roar.DependencyInjection.GeneratorTests;

public class GrpcRegistationGeneratorTest
{
    [Fact(DisplayName = "When there is grpc service, it calls MapGrpcService")]
    public async Task Test()
    {
        string consumingNamespace = "ConsumingProject.GrpcServices";

        string source = $@"using Roar.DependencyInjection.Abstractions;

namespace {consumingNamespace};

public class MyGrpcService : IGrpcService
{{
}}
";

        string expectedGenerated = $@"using Microsoft.AspNetCore.Builder;

namespace {consumingNamespace};

public static partial class RoarGeneratedModule
{{
    public static WebApplication MapRoarEndpoints(this WebApplication app)
    {{
        app.MapGrpcService<{consumingNamespace}.MyGrpcService>();

        return app;
    }}
}}
";

        await GeneratorTestHelper.VerifyAsync<DependencyRegistrationGrpcGenerator>(source, expectedGenerated, "RoarGeneratedModuleGrpc.g.cs", consumingNamespace);
    }
}
