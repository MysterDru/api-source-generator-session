using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NorthwindTraders.WebApi.SourceGen;

/// <remarks>
///     This partial class contains the method to read the semantic model and transform
///     it into a <see cref="ControllerToGenerate"/> instance.
///
///     The result of this method must be value type as it will be cached by the
///     incremental generator.
/// </remarks>
public partial class ApiControllerGenerator
{
    private static ControllerToGenerate? TransformSemanticTarget(
        GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        var symbol =
            context.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax) context.TargetNode, cancellationToken);

        // couldn't get symbol info, abort generation
        if (symbol is not INamedTypeSymbol classSymbol) return null;

        var attribute = context.Attributes.Single();
        var nameOverride = attribute.NamedArguments
            .FirstOrDefault(x => x.Key == "Name").Value
            .Value as string;

        // use target class name if no service type is specified
        var serviceTypeValue = attribute.NamedArguments
            .FirstOrDefault(x => x.Key == "ServiceType").Value
            .Value as INamedTypeSymbol ?? classSymbol;

        return new ControllerToGenerate
        {
            Namespace = classSymbol.ContainingNamespace.ToString(),
            ClassName = classSymbol.Name,
            EntityName = nameOverride,
            ServiceTypeFullyQualifiedName = serviceTypeValue.ToDisplayString(),
            Methods =
            [
                // get all interfaces on the depending on how the developer is implementing interfaces
                // our types might not be directly on the symbol
                ..classSymbol.AllInterfaces.Where(IsKnownInterfaceType)
                    .Select(TransformInterfaceMethod)
                    .Where(x => x is not null)
                    .Cast<MethodToGenerate>()
            ]
        };

        bool IsKnownInterfaceType(INamedTypeSymbol interfaceSymbol)
        {
            List<string> knownInterfaces =
            [
                "IGetHandler`1", "IGetHandler`2", "IGetListHandler`1", "IGetListHandler`2",
                "ICreateHandler`2", "IUpdateHandler`1", "IDeleteHandler", "IDeleteHandler`1", "ISaveHandler`1"
            ];

            return knownInterfaces.Contains(interfaceSymbol.MetadataName) &&
                   interfaceSymbol.ContainingNamespace?.ToDisplayString()
                   == "NorthwindTraders.WebApi.SourceGen";
        }

        MethodToGenerate? TransformInterfaceMethod(INamedTypeSymbol arg)
        {
            var interfaceName = arg.MetadataName.Split('`')[0];
            return interfaceName switch
            {
                "IGetHandler" => CreateMethodToGenerate("IGetHandler", arg.TypeArguments),
                "IGetListHandler" => CreateMethodToGenerate("IGetListHandler", arg.TypeArguments),
                "ICreateHandler" => CreateMethodToGenerate("ICreateHandler", arg.TypeArguments),
                "IUpdateHandler" => CreateMethodToGenerate("IUpdateHandler", arg.TypeArguments),
                "IDeleteHandler" => CreateMethodToGenerate("IDeleteHandler", arg.TypeArguments),
                "ISaveHandler" => CreateMethodToGenerate("ISaveHandler", arg.TypeArguments),
                _ => null
            };
        }

        MethodToGenerate CreateMethodToGenerate(string interfaceName, ImmutableArray<ITypeSymbol> typeArguments)
        {
            return new MethodToGenerate
            {
                InterfaceName = interfaceName,
                QualifiedTypeParameters =
                [
                    ..typeArguments.Select(t => t.ToDisplayString())
                ]
            };
        }
    }
}