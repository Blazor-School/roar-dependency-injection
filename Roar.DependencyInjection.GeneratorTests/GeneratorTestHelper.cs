using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Roar.DependencyInjection.Abstractions;

namespace Roar.DependencyInjection.GeneratorTests;

public class GeneratorTestHelper
{
    public static Task VerifyAsync<T>(string source, string expectedGenerated, string generatedClassName) where T : IIncrementalGenerator, new()
        => new CSharpSourceGeneratorTest<T, DefaultVerifier>
        {
            TestState =
            {
                Sources = { source },
                ReferenceAssemblies = new(targetFramework: "net10.0",
                referenceAssemblyPackage: new PackageIdentity("Microsoft.NETCore.App.Ref", "10.0.0"),
                referenceAssemblyPath: Path.Combine("ref", "net10.0")),
                AdditionalReferences =
                {
                    typeof(IScopedService).Assembly,
                    typeof(Microsoft.Extensions.DependencyInjection.IServiceCollection).Assembly,
                    typeof(Microsoft.AspNetCore.Builder.WebApplication).Assembly,
                    typeof(Microsoft.AspNetCore.Builder.GrpcEndpointRouteBuilderExtensions).Assembly,
                    typeof(Microsoft.AspNetCore.Routing.IEndpointRouteBuilder).Assembly,
                    typeof(Microsoft.AspNetCore.Builder.IApplicationBuilder).Assembly,
                    typeof(Microsoft.Extensions.Hosting.IHost).Assembly,
                },
                GeneratedSources =
                {
                    (typeof(T), generatedClassName, expectedGenerated),
                }
            }
        }.RunAsync();
}
