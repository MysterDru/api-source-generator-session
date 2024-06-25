namespace NorthwindTraders.WebApi.SourceGen.Models;

#nullable disable
public sealed record ControllerToGenerate
{
    public string Namespace { get; init; }

    public string ClassName { get; init; }

    public string EntityName { get; init; }

    public string ServiceTypeFullyQualifiedName { get; init; }

    public ValueEqualityList<MethodToGenerate> Methods { get; init; }
}

public sealed record MethodToGenerate
{
    public string InterfaceName { get; init; }

    public ValueEqualityList<string> QualifiedTypeParameters { get; init; }
}
#nullable enable