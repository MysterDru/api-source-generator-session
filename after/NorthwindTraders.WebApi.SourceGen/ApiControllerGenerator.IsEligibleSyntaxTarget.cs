using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NorthwindTraders.WebApi.SourceGen;

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