namespace NorthwindTraders.WebApi.SourceGen;

/// <summary>
///     This partial class contains the primary entry point and
///     implementation of the generator.
/// </summary>
[Generator]
public partial class ApiControllerGenerator : IIncrementalGenerator
{
    /// <summary>
    ///     The initialize method is broken into 3 steps:
    ///     1. Add the marker attribute and interfaces to the referencing project
    ///     2. Find classes annotated with the [GenerateController] attribute and create
    ///         a value type containing the metadata needed to generate the controller
    ///     3. Generate the source code for the controller
    /// </summary>
    /// <param name="context"></param>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Step 1
        context.RegisterPostInitializationOutput(static initializationContext =>
        {
            // add [GenerateController] attribute
            AddMarkerAttribute(initializationContext);
            
            // add IHandler interfaces
            AddMarkerInterfaces(initializationContext);
        });

        // Step 2
        var pipeline = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: "NorthwindTraders.WebApi.SourceGen.GenerateControllerAttribute",
                predicate: IsEligibleSyntaxNode,
                transform: TransformSemanticTarget)
            .Where(static x => x is not null)
            .Select(static (x, _) => x!);

        // Step 3
        context.RegisterSourceOutput(pipeline, GenerateController);
    }
}