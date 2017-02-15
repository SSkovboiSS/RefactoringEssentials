using System.Collections.Generic;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using RefactoringEssentials.CSharp.Diagnostics;
using System;
using System.Linq;

namespace RefactoringEssentials.CSharp.CodeRefactorings
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = "Use object initializer")]
    public class UseObjectInitializerCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var document = context.Document;
            if (document.Project.Solution.Workspace.Kind == WorkspaceKind.MiscellaneousFiles)
                return;
            var span = context.Span;
            if (!span.IsEmpty)
                return;
            var cancellationToken = context.CancellationToken;
            if (cancellationToken.IsCancellationRequested)
                return;
            var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            if (model.IsFromGeneratedCode(cancellationToken))
                return;
            var root = await model.SyntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false);
            var token = root.FindToken(span.Start);
            var objectCreation = token.Parent as ObjectCreationExpressionSyntax;
            if (objectCreation == null)
                return;
            var creatorStatement = objectCreation.Parent?.Parent;
            var identifier = (creatorStatement as VariableDeclaratorSyntax)?.Identifier.ToString() ??
                (creatorStatement as ExpressionStatementSyntax)?.Expression.ToString();
            if (String.IsNullOrEmpty(identifier))
                return;
            var block = creatorStatement.Ancestors().OfType<BlockSyntax>().FirstOrDefault();
            if (block == null)
                return;

            var afterObjectCreationStatements = block.Statements.SkipWhile(s => s != creatorStatement).Skip(1);
            var nextStatements = new List<ExpressionStatementSyntax>();
            
            foreach(var statement in afterObjectCreationStatements)
            {
                var assignmentExpression = (statement as ExpressionStatementSyntax)?.Expression as AssignmentExpressionSyntax;
                var memberAccess = assignmentExpression?.Left;// as SimpleMemberAccessExpression;
                if (memberAccess == null)
                    break;
            }
            /*var switchExpr = ConvertIfStatementToSwitchStatementAnalyzer.GetSwitchExpression(model, node.Condition);
            if (switchExpr == null)
                return;

            var switchSections = new List<SwitchSectionSyntax>();
            if (!ConvertIfStatementToSwitchStatementAnalyzer.CollectSwitchSections(switchSections, model, node, switchExpr))
                return;

            context.RegisterRefactoring(
                CodeActionFactory.Create(
                    span,
                    DiagnosticSeverity.Info,
                    GettextCatalog.GetString("To 'switch'"),
                    ct =>
                    {
                        var switchStatement = SyntaxFactory.SwitchStatement(switchExpr, new SyntaxList<SwitchSectionSyntax>().AddRange(switchSections));
                        return Task.FromResult(document.WithSyntaxRoot(root.ReplaceNode(
                            (SyntaxNode)node, switchStatement
                            .WithLeadingTrivia(node.GetLeadingTrivia())
                            .WithAdditionalAnnotations(Formatter.Annotation))));
                    })
            );*/
            throw new ArgumentException();
        }
    }
}
