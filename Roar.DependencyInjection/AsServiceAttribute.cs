namespace Roar.DependencyInjection;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
#pragma warning disable CS9113 // Parameter is read in the generator.
public sealed class AsServiceAttribute(Type ServiceType) : Attribute
#pragma warning restore CS9113 // Parameter is read in the generator.
{
}
