using Microsoft.CodeAnalysis;
using System.Text;

namespace Roar.DependencyInjection.Generators.Models;

public class ServiceDefinition(INamedTypeSymbol symbol)
{
    public ServiceLifetime? Lifetime { get; private set; }
    public List<ITypeSymbol> Interfaces { get; private set; } = [];
    public bool ValidService { get; private set; } = false;
    public INamedTypeSymbol Symbol { get; set; } = symbol;

    public static ServiceDefinition GetDefinition(INamedTypeSymbol symbol, INamedTypeSymbol scopedSymbol, INamedTypeSymbol singletonSymbol, INamedTypeSymbol transientSymbol, INamedTypeSymbol backgroundWorkerSymbol, INamedTypeSymbol asServiceSymbol, INamedTypeSymbol grpcServiceSymbol, SourceProductionContext spc)
    {
        var serviceDefinition = new ServiceDefinition(symbol);
        bool validService = true;

        foreach (var attribute in symbol.GetAttributes())
        {
            DefineLifetime(attribute);
            DefineInterfaces(attribute);
        }

        serviceDefinition.ValidService = validService;

        return serviceDefinition;

        void DefineInterfaces(AttributeData attribute)
        {
            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, asServiceSymbol))
            {
                if (attribute.ConstructorArguments.Length == 1 && attribute.ConstructorArguments[0].Value is ITypeSymbol type)
                {
                    if (serviceDefinition.Lifetime is ServiceLifetime.Background)
                    {
                        var diagnostic = Diagnostic.Create(RoarDiagnosticDescriptors.ROAR004, symbol.Locations.FirstOrDefault(), symbol.Name);
                        spc.ReportDiagnostic(diagnostic);
                        validService = false;
                    }

                    if (type.TypeKind != TypeKind.Interface)
                    {
                        var diagnostic = Diagnostic.Create(RoarDiagnosticDescriptors.ROAR001, symbol.Locations.FirstOrDefault(), type.ToDisplayString());
                        spc.ReportDiagnostic(diagnostic);
                        validService = false;
                    }

                    if (!symbol.AllInterfaces.Contains(type, SymbolEqualityComparer.Default))
                    {
                        var diagnostic = Diagnostic.Create(RoarDiagnosticDescriptors.ROAR003, symbol.Locations.FirstOrDefault(), symbol.Name, type.ToDisplayString());
                        spc.ReportDiagnostic(diagnostic);
                        validService = false;
                    }

                    serviceDefinition.Interfaces.Add(type);
                }
            }
        }

        void DefineLifetime(AttributeData attribute)
        {
            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, scopedSymbol))
            {
                ReportWhenMultipleLifetimeViolated();
                serviceDefinition.Lifetime = ServiceLifetime.Scoped;
            }

            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, singletonSymbol))
            {
                ReportWhenMultipleLifetimeViolated();
                serviceDefinition.Lifetime = ServiceLifetime.Singleton;
            }

            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, transientSymbol))
            {
                ReportWhenMultipleLifetimeViolated();
                serviceDefinition.Lifetime = ServiceLifetime.Transient;
            }

            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, backgroundWorkerSymbol))
            {
                ReportWhenMultipleLifetimeViolated();
                serviceDefinition.Lifetime = ServiceLifetime.Background;
            }

            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, grpcServiceSymbol))
            {
                ReportWhenMultipleLifetimeViolated();
                serviceDefinition.Lifetime = ServiceLifetime.Grpc;
            }
        }

        void ReportWhenMultipleLifetimeViolated()
        {
            if (serviceDefinition.Lifetime is not null)
            {
                var diagnostic = Diagnostic.Create(RoarDiagnosticDescriptors.ROAR002, symbol.Locations.FirstOrDefault(), symbol.Name);
                spc.ReportDiagnostic(diagnostic);
                validService = false;
            }
        }
    }

    public string RenderRegister()
    {
        if (ValidService)
        {
            if (Interfaces.Count is 0)
            {
                return Lifetime switch
                {
                    ServiceLifetime.Singleton => $"        services.AddSingleton<{Symbol.ToDisplayString()}>();",
                    ServiceLifetime.Scoped => $"        services.AddScoped<{Symbol.ToDisplayString()}>();",
                    ServiceLifetime.Transient => $"        services.AddTransient<{Symbol.ToDisplayString()}>();",
                    ServiceLifetime.Background => $$"""
        services.AddSingleton<{{Symbol.ToDisplayString()}}>();
        services.AddHostedService<{{Symbol.ToDisplayString()}}>();            
""",
                    _ => ""
                };
            }
            else
            {
                var stringBuilder = new StringBuilder();

                switch (Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        foreach (var interfaceType in Interfaces)
                        {
                            stringBuilder.AppendLine($"        services.AddSingleton<{interfaceType.ToDisplayString()}, {Symbol.ToDisplayString()}>();");
                        }

                        break;
                    case ServiceLifetime.Scoped:
                        foreach (var interfaceType in Interfaces)
                        {
                            stringBuilder.AppendLine($"        services.AddScoped<{interfaceType.ToDisplayString()}, {Symbol.ToDisplayString()}>();");
                        }

                        break;

                    case ServiceLifetime.Transient:
                        foreach (var interfaceType in Interfaces)
                        {
                            stringBuilder.AppendLine($"        services.AddTransient<{interfaceType.ToDisplayString()}, {Symbol.ToDisplayString()}>();");
                        }

                        break;
                    default:
                        break;
                }

                return stringBuilder.ToString();
            }
        }

        return string.Empty;
    }

    public string RenderGrpcMapping() => ValidService && Lifetime == ServiceLifetime.Grpc ? $"        app.MapGrpcService<{Symbol.ToDisplayString()}>();" : string.Empty;
}
