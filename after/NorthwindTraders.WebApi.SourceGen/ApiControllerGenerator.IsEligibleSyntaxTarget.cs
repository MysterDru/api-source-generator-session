using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NorthwindTraders.WebApi.SourceGen;

/// <summary>
///     This partial class determines if a syntax node is eligible to be
///     processed by the source generator.
/// </summary>
public partial class ApiControllerGenerator
{
    private static bool IsEligibleSyntaxNode(SyntaxNode node, CancellationToken cancellationToken)
    {
        if (node is not ClassDeclarationSyntax classDeclaration) return false;

        return !classDeclaration.Modifiers.Any(modifier =>
            modifier.IsKind(SyntaxKind.AbstractKeyword)
            || modifier.IsKind(SyntaxKind.StaticKeyword));
    }
}