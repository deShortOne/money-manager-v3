using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MoneyTracker.InterceptDoubleNegativeResult;

/// <summary>
/// Finds usages of double negative when it comes to Result pattern and suggests to not use result pattern
/// i.e. !Result.IsSuccess => Result.HasError
/// i.e. !Result.HasError => Result.IsSuccess
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class InterceptDoubleNegativeResultAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "MT0001";

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.MT0001Title),
        Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.MT0001MessageFormat),
        Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.MT0001Description),
        Resources.ResourceManager, typeof(Resources));

    private const string Category = "Usage";

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.LogicalNotExpression);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        // Ensure it's unary expression // might need to check for Result.IsSuccess == false
        if (context.Node is not PrefixUnaryExpressionSyntax unaryExpression)
            return;

        // Ensure it's !<operand>
        if (unaryExpression.Operand is not MemberAccessExpressionSyntax memberAccess)
            return;

        // Check that the member name is IsSuccess or HasError
        var isIsSuccessField = memberAccess.Name.Identifier.Text == "IsSuccess";
        var isHasErrorField = memberAccess.Name.Identifier.Text == "HasError";
        if (!isIsSuccessField && !isHasErrorField)
            return;

        var symbolInfo = context.SemanticModel.GetSymbolInfo(memberAccess.Expression, context.CancellationToken);
        if (symbolInfo.Symbol == null)
            return;

        if (symbolInfo.Symbol is ILocalSymbol localSymbol && localSymbol.Type.Name == "Result" ||
            symbolInfo.Symbol is IFieldSymbol fieldSymbol && fieldSymbol.Type.Name == "Result" ||
            symbolInfo.Symbol is IPropertySymbol propertySymbol && propertySymbol.Type.Name == "Result")
        {
            var diagnostic = isIsSuccessField
                ? Diagnostic.Create(Rule,
                    unaryExpression.GetLocation(),
                    "IsSuccess", "HasError")
                : Diagnostic.Create(Rule,
                    unaryExpression.GetLocation(),
                    "HasError", "IsSuccess");

            context.ReportDiagnostic(diagnostic);
        }
    }
}
