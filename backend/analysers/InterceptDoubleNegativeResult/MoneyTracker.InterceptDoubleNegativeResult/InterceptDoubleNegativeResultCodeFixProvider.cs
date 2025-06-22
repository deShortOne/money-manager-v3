using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace MoneyTracker.InterceptDoubleNegativeResult;

/// <summary>
/// A sample code fix provider that renames classes with the company name in their definition.
/// All code fixes must  be linked to specific analyzers.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InterceptDoubleNegativeResultCodeFixProvider)), Shared]
public class InterceptDoubleNegativeResultCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(InterceptDoubleNegativeResultAnalyzer.DiagnosticId);

    // If you don't need the 'fix all' behaviour, return null.
    public override FixAllProvider? GetFixAllProvider() => null; //FixAllProvider.Create();

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.Single();
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnosticNode = root?.FindNode(diagnosticSpan);

        if (diagnosticNode is not PrefixUnaryExpressionSyntax unaryExpression)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(
                title: string.Format(Resources.MT0001Title, "IsSuccess", "HasError"),
                createChangedDocument: c => RemoveDoubleNegativeFromResultClass(context.Document, unaryExpression, c),
                equivalenceKey: nameof(Resources.MT0001Title)),
            diagnostic);
    }

    /// <summary>
    /// Executed on the quick fix action raised by the user.
    /// </summary>
    /// <param name="document">Affected source file.</param>
    /// <param name="unaryExpression"></param>
    /// <param name="cancellationToken">Any fix is cancellable by the user, so we should support the cancellation token.</param>
    /// <returns>Clone of the solution with updates: renamed class.</returns>
    private async Task<Document> RemoveDoubleNegativeFromResultClass(Document document,
        PrefixUnaryExpressionSyntax unaryExpression, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var root = await document.GetSyntaxRootAsync(cancellationToken)
            .ConfigureAwait(false);
        if (root is null)
            return document;

        if (unaryExpression.Operand is not MemberAccessExpressionSyntax oldMemberAccess)
            return document;
        var isIsSuccessField = oldMemberAccess.Name.Identifier.Text == "IsSuccess";

        var generator = SyntaxGenerator.GetGenerator(document);
        var newMemberAccess = generator.MemberAccessExpression(
                oldMemberAccess.Expression,
                isIsSuccessField ? "HasError" : "IsSuccess")
            .WithTriviaFrom(oldMemberAccess);

        var newRoot = root.ReplaceNode(unaryExpression, newMemberAccess);

        return document.WithSyntaxRoot(newRoot);
    }
}
