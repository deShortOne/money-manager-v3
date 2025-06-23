using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MoneyTracker.InterceptDoubleNegativeResult
{
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

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
            DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction((syntaxNodeAnalysisContext) => ForClassXInvertNotMethodYToMethodZ(syntaxNodeAnalysisContext, "Result", "IsSuccess", "HasError"), SyntaxKind.LogicalNotExpression);
            context.RegisterSyntaxNodeAction((syntaxNodeAnalysisContext) => ForClassXInvertNotMethodYToMethodZ(syntaxNodeAnalysisContext, "Result", "HasError", "IsSuccess"), SyntaxKind.LogicalNotExpression);
            context.RegisterSyntaxNodeAction((syntaxNodeAnalysisContext) => ForClassXInvertNotMethodYToMethodZ(syntaxNodeAnalysisContext, "ResultT", "IsSuccess", "HasError"), SyntaxKind.LogicalNotExpression);
            context.RegisterSyntaxNodeAction((syntaxNodeAnalysisContext) => ForClassXInvertNotMethodYToMethodZ(syntaxNodeAnalysisContext, "ResultT", "HasError", "IsSuccess"), SyntaxKind.LogicalNotExpression);
        }

        private void ForClassXInvertNotMethodYToMethodZ(SyntaxNodeAnalysisContext context, string className, string methodY, string methodZ)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            // Ensure it's unary expression // might need to check for Result.IsSuccess == false
            if (!(context.Node is PrefixUnaryExpressionSyntax unaryExpression))
                return;

            // Ensure it's !<operand>
            if (!(unaryExpression.Operand is MemberAccessExpressionSyntax memberAccess))
                return;

            // Check that the member name is IsSuccess or HasError
            if (memberAccess.Name.Identifier.Text != methodY)
                return;

            var symbolInfo = context.SemanticModel.GetSymbolInfo(memberAccess.Expression, context.CancellationToken);
            if (symbolInfo.Symbol == null)
                return;

            if (symbolInfo.Symbol is ILocalSymbol localSymbol && localSymbol.Type.Name == className ||
                symbolInfo.Symbol is IFieldSymbol fieldSymbol && fieldSymbol.Type.Name == className ||
                symbolInfo.Symbol is IPropertySymbol propertySymbol && propertySymbol.Type.Name == className)
            {
                var fromCode = $"!{symbolInfo.Symbol.Name}.{methodY}";
                var toCode = $"{symbolInfo.Symbol.Name}.{methodZ}";

                var diagnostic = Diagnostic.Create(Rule,
                        unaryExpression.GetLocation(),
                        fromCode, toCode);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
