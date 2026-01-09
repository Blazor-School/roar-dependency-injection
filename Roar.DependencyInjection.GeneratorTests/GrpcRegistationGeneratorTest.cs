using Roar.DependencyInjection.Generators;

namespace Roar.DependencyInjection.GeneratorTests;

public class GrpcRegistationGeneratorTest
{
    [Fact(DisplayName = "When there is grpc service, it calls MapGrpcService")]
    public async Task Test()
    {
        const string source = """
            using Roar.DependencyInjection.Abstractions;

            namespace ConsumingProject.GrpcServices;

            public class MyGrpcService : IGrpcService
            {
            }
            """;
        const string expectedGenerated = """
            using Microsoft.AspNetCore.Builder;

            namespace Roar.DependencyInjection.Generated;

            public static partial class RoarGeneratedModule
            {
                public static WebApplication MapRoarEndpoints(this WebApplication app)
                {
                   app.MapGrpcService<ConsumingProject.GrpcServices.MyGrpcService>();

                    return app;
                }
            }

            """;

        await GeneratorTestHelper.VerifyAsync<DependencyRegistrationGrpcGenerator>(source, expectedGenerated, "RoarGeneratedModuleGrpc.g.cs");
    }
}
