namespace NorthwindTraders.WebApi.SourceGen;

[Generator]
public partial class ApiControllerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Step 1: Add the marker attribute and interfaces to the referencing project:
        context.RegisterPostInitializationOutput(static initializationContext =>
        {
            // add [GenerateController] attribute
            AddMarkerAttribute(initializationContext);
            
            // add IHandler interfaces
            AddMarkerInterfaces(initializationContext);
        });

        // Step 2: When the generator is triggered by the compilation, find classes annotated with the [GenerateController] attribute
        var pipeline = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "NorthwindTraders.WebApi.SourceGen.GenerateControllerAttribute",

                // ensure that the node is a concrete class
                IsEligibleSyntaxNode,

                // transform the syntax node into value type containing the metadata needed to generate the controller
                // this result must be a value type, as it will be cached by the generator via IEquatable
                TransformSemanticTarget)
            .Where(static x => x is not null)
            .Select(static (x, _) => x!);

        // Step 3: For any classes that are found, generate the source code for the controller
        context.RegisterSourceOutput(pipeline, GenerateController);
    }
}