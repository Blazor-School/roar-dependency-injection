using Microsoft.CodeAnalysis;

namespace Roar.DependencyInjection.Generators.Models;

// Please define the diagnostics in both AnalyzerReleases.Unshipped.md and AnalyzerReleases.Shipped.md
public static class RoarDiagnosticDescriptors
{
    public static DiagnosticDescriptor ROAR001 = new("ROAR001", "Invalid AsService target", "Type '{0}' in [AsService] must be an interface", "Roar.DependencyInjection", DiagnosticSeverity.Error, true);
    public static DiagnosticDescriptor ROAR002 = new("ROAR002", "Multiple lifetime attributes", "Class '{0}' has more than one lifetime attribute. Only one of ScopedService, SingletonService, TransientService, BackgroundWorker, GrpcService is allowed.", "Roar.DependencyInjection", DiagnosticSeverity.Error, true);
    public static DiagnosticDescriptor ROAR003 = new("ROAR003", "Interface not implemented", "Class '{0}' does not implement interface '{1}' specified in [AsService]", "Roar.DependencyInjection", DiagnosticSeverity.Error, true);
    public static DiagnosticDescriptor ROAR004 = new("ROAR004", "Background Worker does not have life time", "Class '{0}' is background worker but registered with [AsService]", "Roar.DependencyInjection", DiagnosticSeverity.Error, true);
}
