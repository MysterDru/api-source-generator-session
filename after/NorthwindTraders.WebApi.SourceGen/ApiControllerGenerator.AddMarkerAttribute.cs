namespace NorthwindTraders.WebApi.SourceGen;

/// <remarks>
///		This partial class contains the logic to generate the <c>GenerateControllerAttribute</c> attribute.
/// </remarks>
public partial class ApiControllerGenerator
{
    private static void AddMarkerAttribute(IncrementalGeneratorPostInitializationContext postInitializationContext)
    {
        var hintName = "NorthwindTraders.WebApi.SourceGen.GenerateControllerAttribute.g.cs";

        var attributeSource = @"namespace NorthwindTraders.WebApi.SourceGen;
#nullable enable
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class GenerateControllerAttribute : Attribute
{
	/// <summary>
	/// Gets or sets the name of the controller to generate, without the ""Controller"" suffix.
	/// If not specified, the class name will be used.
	/// </summary>
	public string? Name { get; set; }
	
	/// <summary>
	/// Gets or sets the service type to inject into the controller. If not specified, the
	/// current class type will be used.
	/// </summary>
	public Type? ServiceType { get; set; }
}
#nullable disable";

        // add the source text to the compilation
        postInitializationContext.AddSource(hintName, attributeSource);
    }
}