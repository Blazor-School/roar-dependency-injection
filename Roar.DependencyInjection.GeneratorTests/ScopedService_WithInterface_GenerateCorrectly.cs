namespace Roar.DependencyInjection.GeneratorTests;

public class ScopedService_WithInterface_GenerateCorrectly
{
    [Fact]
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
            using Microsoft.AspNetCore.Builder;
            using Microsoft.Extensions.DependencyInjection;

            namespace Roar.DependencyInjection.Generated;

            public static class RoarGeneratedModule
            {
                public static IServiceCollection AddRoarServices(this IServiceCollection services)
                {
                    services.AddScoped<ConsumingProject.Services.MyScopedService>();

                    return services;
                }

                public static WebApplication MapRoarEndpoints(this WebApplication app)
                {

                    return app;
                }
            }

            """;

        await GeneratorTestHelper.VerifyAsync(source, expectedGenerated);
    }
}
