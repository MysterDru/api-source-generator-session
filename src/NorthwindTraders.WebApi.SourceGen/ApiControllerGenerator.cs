using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Schema;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NorthwindTraders.WebApi.SourceGen.Models;

namespace NorthwindTraders.WebApi.SourceGen;

[Generator]
public class ApiControllerGenerator : IIncrementalGenerator
{
    private const string GenerateControllerAttributeMetadataName =
        "NorthwindTraders.WebApi.SourceGen.GenerateControllerAttribute";

    private const string GenerateControllerAttributeClassName =
        "GenerateController";

    private const string GenerateControllerAttributeTypeName =
        "GenerateControllerAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Step 1: Add the marker attribute to the referencing project: [GenerateController]
        context.RegisterPostInitializationOutput(GenerateMarkers);

        // Step 2: When the generator is triggered by the compilation, find classes annotated with the [GenerateController] attribute
        var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
                // find any node in the syntax tree that is decorated with the GenerateControllerAttribute
                GenerateControllerAttributeMetadataName,

                // ensure that the node is a concrete class
                predicate: static (s, _) => IsSyntaxNodeConcreteClass(s),

                // transform the syntax node into value type containing the metadata needed to generate the controller
                // this result must be a value type, as it will be cached by the generator via IEquatable
                transform: TransformSemanticTarget)
            .Where(static x => x is not null);

        // Step 3: For any classes that are found, generate the source code for the controller
        context.RegisterSourceOutput(pipeline, static (ctx, x) => GenerateController(ctx, x!));
    }

    private static bool IsSyntaxNodeConcreteClass(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDeclaration
               && !classDeclaration.Modifiers.Any(SyntaxKind.AbstractKeyword)
               && !classDeclaration.Modifiers.Any(SyntaxKind.StaticKeyword);
    }

    private static ControllerToGenerate? TransformSemanticTarget(
        GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        INamedTypeSymbol? classSymbol =
            context.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax) context.TargetNode, cancellationToken);

        // couldn't get symbol info, abort generation
        if (classSymbol is null)
        {
            return null;
        }

        var attribute = FindAttribute(classSymbol);
        var nameOverride = attribute.NamedArguments
            .FirstOrDefault(x => x.Key == "Name").Value
            .Value as string;

        // use target class name if no service type is specified
        var serviceTypeValue = attribute.NamedArguments
            .FirstOrDefault(x => x.Key == "ServiceType").Value
            .Value as INamedTypeSymbol ?? classSymbol;
        
        return new ControllerToGenerate()
        {
            Namespace = classSymbol.ContainingNamespace.ToString(),
            ClassName = classSymbol.Name,
            EntityName = nameOverride,
            ServiceTypeFullyQualifiedName = serviceTypeValue.ToDisplayString(),
            Methods = [
                // get all interfaces on the depending on how the developer is implementing interfaces
                // our types might not be directly on the symbol
                ..classSymbol.AllInterfaces.Where(IsKnownInterfaceType)
                    .Select(TransformInterfaceMethod)
                    .Where(x => x is not null)
                    .Cast<MethodToGenerate>()
            ]
        };
    }

    private static bool IsKnownInterfaceType(INamedTypeSymbol arg)
    {
        List<string> knownInterfaces =
        [
            "IGetHandler`1", "IGetListHandler`1",
            "ICreateHandler`2", "IUpdateHandler`1", "IDeleteHandler", "ISaveHandler`1"
        ];

        return knownInterfaces.Contains(arg.MetadataName) && arg is
        {
            ContainingNamespace:
            {
                Name: "SourceGen",
                ContainingNamespace:
                {
                    Name: "WebApi",
                    ContainingNamespace:
                    {
                        Name: "NorthwindTraders"
                    }
                }
            }
        };
    }

    private static MethodToGenerate? TransformInterfaceMethod(INamedTypeSymbol arg)
    {
        return arg.MetadataName switch
        {
            "IGetHandler`1" => new MethodToGenerate()
            {
                InterfaceName = "IGetHandler",
                QualifiedTypeParameters =
                [
                    arg.TypeArguments[0].ToDisplayString()
                ]
            },
            "IGetListHandler`1" => new MethodToGenerate()
            {
                InterfaceName = "IGetListHandler",
                QualifiedTypeParameters =
                [
                    arg.TypeArguments[0].ToDisplayString()
                ]
            },
            "ICreateHandler`2" => new MethodToGenerate()
            {
                InterfaceName = "ICreateHandler",
                QualifiedTypeParameters =
                [
                    arg.TypeArguments[0].ToDisplayString(),
                    arg.TypeArguments[1].ToDisplayString()
                ]
            },
            "IUpdateHandler`1" => new MethodToGenerate()
            {
                InterfaceName = "IUpdateHandler",
                QualifiedTypeParameters =
                [
                    arg.TypeArguments[0].ToDisplayString()
                ]
            },
            "IDeleteHandler" => new MethodToGenerate()
            {
                InterfaceName = "IDeleteHandler",
                QualifiedTypeParameters = []
            },
            "ISaveHandler`1" => new MethodToGenerate()
            {
                InterfaceName = "ISaveHandler",
                QualifiedTypeParameters =
                [
                    arg.TypeArguments[0].ToDisplayString()
                ]
            },
            _ => null
        };
    }


    private static AttributeData FindAttribute(INamedTypeSymbol classSymbol)
        => classSymbol.GetAttributes()
            .Single(a => a?.AttributeClass is
            {
                Name: GenerateControllerAttributeClassName or GenerateControllerAttributeTypeName,
                ContainingNamespace:
                {
                    Name: "SourceGen",
                    ContainingNamespace:
                    {
                        Name: "WebApi",
                        ContainingNamespace:
                        {
                            Name: "NorthwindTraders"
                        }
                    }
                }
            });

    private static void GenerateController(SourceProductionContext context,
        ControllerToGenerate generateInfo)
    {
        var sourceText = new StringBuilder("/// <auto-generated />\n")
            .AppendLine("using Microsoft.AspNetCore.Mvc;")
            .AppendLine("using System;")
            .AppendLine("using System.Collections.Generic;")
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine()
            .AppendLine($"namespace {generateInfo.Namespace};")
            .AppendLine()
            .AppendLine("[ApiController]")
            .AppendLine(
                $"public class {generateInfo.ClassName}Controller({generateInfo.ServiceTypeFullyQualifiedName} service) : ControllerBase")
            .AppendLine("{");

        foreach (var method in generateInfo.Methods)
        {
            sourceText.AppendLine();

            if (method.InterfaceName == "IGetHandler")
            {
                sourceText
                    .AppendLine($"\t[HttpGet(\"/api/{generateInfo.EntityName}.get\")]")
                    .AppendLine($"\tpublic async Task<ActionResult<{method.QualifiedTypeParameters[0]}>> Get(int id)")
                    .AppendLine("\t\t=> Ok(await service.GetAsync(id, default));");
            }
            else if (method.InterfaceName == "IGetListHandler")
            {
                sourceText.AppendLine($"\t[HttpGet(\"/api/{generateInfo.EntityName}.list\")]")
                    .AppendLine(
                        $"\tpublic async Task<ActionResult<IReadOnlyList<{method.QualifiedTypeParameters[0]}>>> GetList()")
                    .AppendLine("\t\t=> Ok(await service.GetListAsync(default));");
            }
            else if (method.InterfaceName == "ICreateHandler")
            {
                sourceText.AppendLine($"\t[HttpPost(\"/api/{generateInfo.EntityName}.create\")]")
                    .AppendLine(
                        $"\tpublic async Task<ActionResult<{method.QualifiedTypeParameters[1]}>> Create({method.QualifiedTypeParameters[0]} request)")
                    .AppendLine("\t{")
                    .AppendLine("\t\tvar id = await service.CreateAsync(request, default);")
                    .AppendLine("\t\treturn Ok(await service.GetAsync(id, default));")
                    .AppendLine("\t}");
            }
            else if (method.InterfaceName == "IUpdateHandler")
            {
                sourceText.AppendLine($"\t[HttpPost(\"/api/{generateInfo.EntityName}.update\")]")
                    .AppendLine(
                        $"\tpublic async Task<NoContentResult> Update(int id, {method.QualifiedTypeParameters[0]} request)")
                    .AppendLine("\t{")
                    .AppendLine("\t\tawait service.UpdateAsync(id, request, default);")
                    .AppendLine("\t\treturn NoContent();")
                    .AppendLine("\t}");
            }
            else if (method.InterfaceName == "IDeleteHandler")
            {
                sourceText.AppendLine($"\t[HttpPost(\"/api/{generateInfo.EntityName}.delete\")]")
                    .AppendLine($"\tpublic async Task<NoContentResult> Delete(int id)")
                    .AppendLine("\t{")
                    .AppendLine("\t\tawait service.DeleteAsync(id, default);")
                    .AppendLine("\t\treturn NoContent();")
                    .AppendLine("\t}");
            }
            else if (method.InterfaceName == "ISaveHandler")
            {
                sourceText.AppendLine($"\t[HttpPost(\"/api/{generateInfo.EntityName}.save\")]")
                    .AppendLine($"\tpublic async Task<NoContentResult> Save({method.QualifiedTypeParameters[0]} request)")
                    .AppendLine("\t{")
                    .AppendLine("\t\tawait service.SaveAsync(request, default);")
                    .AppendLine("\t\treturn NoContent();")
                    .AppendLine("\t}");
            }
        }

        sourceText.AppendLine("}");

        context.AddSource($"{generateInfo.ClassName}Controller.g.cs", sourceText.ToString());
    }

    private static void GenerateMarkers(IncrementalGeneratorPostInitializationContext postInitializationContext)
    {
        // define the source text of the attribute code
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
        postInitializationContext.AddSource("NorthwindTraders.WebApi.SourceGen.GenerateControllerAttribute.g.cs",
            attributeSource);

        // define the source of the common interfaces
        var interfacesSource = @"using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NorthwindTraders.WebApi.SourceGen;

public interface IGetHandler<TResponse>
    where TResponse : class
{
    Task<TResponse> GetAsync(int id, CancellationToken cancellationToken);
}

public interface IGetListHandler<TResponse> where TResponse : class
{
    Task<IReadOnlyList<TResponse>> GetListAsync(CancellationToken cancellationToken);
}

public interface ICreateHandler<in TRequest, TResponse> : IGetHandler<TResponse>
    where TRequest : class
    where TResponse : class
{
    Task<int> CreateAsync(TRequest request, CancellationToken cancellationToken);
}

public interface IUpdateHandler<in TRequest>
    where TRequest : class
{
    Task UpdateAsync(int id, TRequest request, CancellationToken cancellationToken);
}

public interface IDeleteHandler
{
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}

public interface ISaveHandler<in TRequest>
    where TRequest : class
{
    Task SaveAsync(TRequest request, CancellationToken cancellationToken);
}";

        // add the source to the application
        postInitializationContext.AddSource("NorthwindTraders.WebApi.SourceGen.Interfaces.g.cs",
            interfacesSource);
    }
}