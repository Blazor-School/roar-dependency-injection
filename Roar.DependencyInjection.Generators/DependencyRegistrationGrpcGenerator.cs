using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
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

                var grpcServiceInterface = compilation.GetTypeByMetadataName("Roar.DependencyInjection.Abstractions.IGrpcService");

                var sb = new StringBuilder($$"""
using Microsoft.AspNetCore.Builder;

namespace {{ns}};

[global::System.CodeDom.Compiler.GeneratedCode("RoarEngine", "1.0.0")]
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

                    foreach (var i in symbol.AllInterfaces)
                    {
                        if (SymbolEqualityComparer.Default.Equals(i, grpcServiceInterface))
                        {
                            sb.AppendLine($"    app.MapGrpcService<{symbol.ToDisplayString()}>();");
                            break;
                        }
                    }
                }

                sb.AppendLine("""

        return app;
    }
}
""");

                spc.AddSource("RoarGeneratedModuleGrpc.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
            });
    }
}
