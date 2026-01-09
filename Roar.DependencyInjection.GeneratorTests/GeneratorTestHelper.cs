using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Roar.DependencyInjection.Abstractions;
using Roar.DependencyInjection.Generators;

namespace Roar.DependencyInjection.GeneratorTests;

public class GeneratorTestHelper
{
    public static Task VerifyAsync(string source, string expectedGenerated) => new CSharpSourceGeneratorTest<DepedencyRegistrationGenerator, DefaultVerifier>
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
                typeof(Microsoft.AspNetCore.Builder.WebApplication).Assembly
            },
            GeneratedSources =
            {
                (typeof(DepedencyRegistrationGenerator), "RoarGeneratedModule.g.cs", expectedGenerated),
            }
        }
    }.RunAsync();
}
