using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roar.DependencyInjection.Generators.Models;
using System.Text;

namespace Roar.DependencyInjection.Generators;

[Generator]
public class DependencyRegistrationGrpcGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var rootNamespace = context.AnalyzerConfigOptionsProvider
            .Select((options, _) =>
            {
                options.GlobalOptions.TryGetValue("build_property.RootNamespace", out string? ns);
                return ns ?? "Roar.DependencyInjection.Generated"; // Fallback if not found
            });

        var classSymbols =
            context.SyntaxProvider
                .CreateSyntaxProvider(
                    static (node, _) => node is ClassDeclarationSyntax,
                    static (ctx, _) =>
                    {
                        var cls = (ClassDeclarationSyntax)ctx.Node;
                        var symbol = ctx.SemanticModel.GetDeclaredSymbol(cls) as INamedTypeSymbol;
                        return symbol;
                    })
                .Where(static s => s is not null && !s.IsAbstract);

        var compilationAndSymbols = context.CompilationProvider
            .Combine(classSymbols.Collect())
            .Combine(rootNamespace);

        context.RegisterSourceOutput(
            compilationAndSymbols,
            (spc, source) =>
            {
                var ((compilation, symbols), ns) = source;

                var scopedAttribute = compilation.GetTypeByMetadataName("Roar.DependencyInjection.ScopedServiceAttribute");
                var singletonAttribute = compilation.GetTypeByMetadataName("Roar.DependencyInjection.SingletonServiceAttribute");
                var transientAttribute = compilation.GetTypeByMetadataName("Roar.DependencyInjection.TransientServiceAttribute");
                var backgroundWorkerAttribute = compilation.GetTypeByMetadataName("Roar.DependencyInjection.BackgroundWorkerAttribute");
                var grpcServiceAttribute = compilation.GetTypeByMetadataName("Roar.DependencyInjection.GrpcServiceAttribute");
                var asServiceAttribute = compilation.GetTypeByMetadataName("Roar.DependencyInjection.AsServiceAttribute");

                var stringBuilder = new StringBuilder($$"""
using Microsoft.AspNetCore.Builder;

namespace {{ns}};

[global::System.CodeDom.Compiler.GeneratedCode("RoarEngine", "2.0.0")]
[global::System.Diagnostics.DebuggerNonUserCode]
[global::System.Diagnostics.DebuggerStepThrough]
[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
[global::System.Runtime.CompilerServices.CompilerGenerated]
public static class RoarGeneratedModuleGrpc
{
    public static WebApplication MapRoarEndpoints(this WebApplication app)
    {

""");

                foreach (var symbol in symbols)
                {
                    if (symbol is null)
                    {
                        continue;
                    }

                    var serviceDefinition = ServiceDefinition.GetDefinition(symbol, scopedAttribute!, singletonAttribute!, transientAttribute!, backgroundWorkerAttribute!, asServiceAttribute!, grpcServiceAttribute!, spc);
                    string renderedRegisterCode = serviceDefinition.RenderGrpcMapping();

                    if (!string.IsNullOrEmpty(renderedRegisterCode))
                    {
                        stringBuilder.AppendLine(renderedRegisterCode);
                    }
                }

                stringBuilder.AppendLine("""

        return app;
    }
}
""");

                spc.AddSource("RoarGeneratedModuleGrpc.g.cs", SourceText.From(stringBuilder.ToString(), Encoding.UTF8));
            });
    }
}
