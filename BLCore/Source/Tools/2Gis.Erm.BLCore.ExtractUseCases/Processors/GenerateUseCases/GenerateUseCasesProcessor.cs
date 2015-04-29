using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.ExtractUseCases.Generated.UseCases;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.ExtractUseCases;
using DoubleGis.Erm.BLCore.ExtractUseCases.Utils;

using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;
using Roslyn.Services.Formatting;

using UseCase = DoubleGis.Erm.BLCore.ExtractUseCases.UseCases.UseCase;
using UseCaseNode = DoubleGis.Erm.BLCore.ExtractUseCases.UseCases.UseCaseNode;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.GenerateUseCases
{
    public class GenerateUseCasesProcessor : AbstractProcessor
    {
        public GenerateUseCasesProcessor(IProcessingContext processingContext, IWorkspace workspace, ISolution solution)
            : base(processingContext, workspace, solution)
        {
        }

        #region Overrides of AbstractProcessor

        private readonly Type _targetUseCasesFactoryType = typeof(UseCaseList);
        private INamedTypeSymbol _targetUseCasesFactoryTypeSymbol;
        private DocumentId _targetUseCasesFactoryDocumentId;

        public override bool IsDocumentProcessingRequired(ISemanticModel semanticModel, IDocument document)
        {
            if (_targetUseCasesFactoryTypeSymbol == null)
            {
                var targetTypeSymbol = semanticModel.Compilation.GetTypeByMetadataName(_targetUseCasesFactoryType.FullName);
                var declaredClass = document.GetSyntaxRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault(cd => semanticModel.GetDeclaredSymbol(cd).Equals(targetTypeSymbol));
                if (declaredClass != null)
                {
                    _targetUseCasesFactoryTypeSymbol = semanticModel.GetDeclaredSymbol(declaredClass) as INamedTypeSymbol;
                    _targetUseCasesFactoryDocumentId = document.Id;
                }
            }

            return false;
        }

        public override void ProcessDocument(ISemanticModel semanticModel, IDocument document)
        {
            throw new NotImplementedException();
        }

        private readonly Dictionary<string, UsingDirectiveSyntax> _targetUsingList = new Dictionary<string, UsingDirectiveSyntax>();
        private readonly List<string> _commonUsingList = new List<string>
            {
                "System.Collections.Generic",
                "DoubleGis.Erm.ServiceLayer",
                "DoubleGis.Erm.BLCore.Handlers.Generic",
                "DoubleGis.Erm.Core.RequestResponse",
                "DoubleGis.Erm.Model.Metadata.UseCases"
            };

        public override void FinishProcessing()
        {
            var usecases = ProcessingContext.GetValue(ExtractUseCasesKey.Instance);
            
            var useCaseListElements = new List<ExpressionSyntax>();
            var useCaseListElementsSeparators = new List<SyntaxToken>();
            foreach (var useCase in usecases.ProcessedUseCases)
            {
                useCaseListElements.Add(GetUseCaseSyntax(useCase));
                useCaseListElementsSeparators.Add(Syntax.Token(SyntaxKind.CommaToken));
            }

            var expressionsList = Syntax.SeparatedList(useCaseListElements, useCaseListElementsSeparators);
            var originalInitializer = GetTargetInitializerEndpoint();
            var generatedInitializer = originalInitializer.WithExpressions(expressionsList);

            var originalRoot = originalInitializer.SyntaxTree.GetRoot();
            var updatedRoot = originalRoot.ReplaceNodes(new SyntaxNode[] { originalInitializer }, (node, syntaxNode) => generatedInitializer as SyntaxNode);

            // обработка common usings
            foreach (var commonUsing in _commonUsingList)
            {
                if (!_targetUsingList.ContainsKey(commonUsing))
                {
                    _targetUsingList.Add(commonUsing, Syntax.UsingDirective(Syntax.IdentifierName(commonUsing)));
                }
            }

            updatedRoot = updatedRoot.WithUsings(Syntax.List(_targetUsingList.Values.Select(u => u).OrderBy(u => (u.Name as IdentifierNameSyntax).Identifier.Value).ToArray()));
            SaveGeneratedUseCaseList(updatedRoot);
        }

        private void SaveGeneratedUseCaseList(CompilationUnitSyntax updatedSyntaxRoot)
        {
            var formatedRoot = updatedSyntaxRoot.Format(FormattingOptions.GetDefaultOptions()).GetFormattedRoot();

            var targetDocument = Solution.GetDocument(_targetUseCasesFactoryDocumentId);
            if (targetDocument == null)
            {
                throw new InvalidOperationException("Can't find specified document in solution by ID: " + _targetUseCasesFactoryDocumentId);
            }

            var oldSolution = Solution;
            var updatedDocument = targetDocument.UpdateSyntaxRoot(formatedRoot);
            var newSolution = oldSolution.UpdateDocument(updatedDocument);
            var isApplied = Workspace.ApplyChanges(oldSolution, newSolution);
        }

        private InitializerExpressionSyntax GetTargetInitializerEndpoint()
        {
            if (_targetUseCasesFactoryTypeSymbol == null)
            {
                throw new InvalidOperationException("Can't find target type: " + _targetUseCasesFactoryType);
            }

            const string TargetMethodName = "GetUseCases";
            var targetSymbol = _targetUseCasesFactoryTypeSymbol.GetMembers(TargetMethodName).FirstOrDefault() as IMethodSymbol;
            if (targetSymbol == null)
            {
                throw new InvalidOperationException("Can't find target method " + TargetMethodName + " in target type " + _targetUseCasesFactoryType);
            }

            var arrayInitializerNode = targetSymbol.DeclaringSyntaxNodes.First().DescendantNodes().OfType<InitializerExpressionSyntax>().First();
            if (arrayInitializerNode == null)
            {
                throw new InvalidOperationException("Can't find required source code pattern in method " + TargetMethodName);
            }

            return arrayInitializerNode;
        }

        private ObjectCreationExpressionSyntax GetUseCaseSyntax(UseCase useCase)
        {
            return Syntax.ObjectCreationExpression(Syntax.IdentifierName("UseCase")).WithInitializer(
                    Syntax.InitializerExpression(
                        SyntaxKind.ObjectInitializerExpression,
                        Syntax.SeparatedList<ExpressionSyntax>(
                            Syntax.BinaryExpression(
                                SyntaxKind.AssignExpression,
                                Syntax.IdentifierName("Description"),
                                Syntax.LiteralExpression(SyntaxKind.StringLiteralExpression, Syntax.Literal(useCase.Description))),
                            Syntax.Token(SyntaxKind.CommaToken),
                            Syntax.BinaryExpression(
                                SyntaxKind.AssignExpression,
                                Syntax.IdentifierName("MaxUseCaseDepth"),
                                Syntax.LiteralExpression(SyntaxKind.NumericLiteralExpression, Syntax.Literal(useCase.MaxUseCaseDepth))),
                            Syntax.Token(SyntaxKind.CommaToken),
                            Syntax.BinaryExpression(SyntaxKind.AssignExpression, Syntax.IdentifierName("Root"), GetUseCaseNodeSyntax(useCase.Root)))));
        }

        private ObjectCreationExpressionSyntax GetUseCaseNodeSyntax(UseCaseNode useCaseNode)
        {
            var useCaseNodeSyntax = Syntax.ObjectCreationExpression(Syntax.IdentifierName("UseCaseNode"))
                            .WithArgumentList(
                                Syntax.ArgumentList(
                                    Syntax.SeparatedList(Syntax.Argument(Syntax.LiteralExpression(SyntaxKind.NumericLiteralExpression, Syntax.Literal(useCaseNode.Level))))))
                            .WithInitializer(
                                Syntax.InitializerExpression(
                                SyntaxKind.ObjectInitializerExpression,
                                Syntax.SeparatedList<ExpressionSyntax>(
                                    Syntax.BinaryExpression(
                                        SyntaxKind.AssignExpression,
                                        Syntax.IdentifierName("ContainingClass"),
                                        Syntax.TypeOfExpression(Syntax.IdentifierName(useCaseNode.ContainingClass.TypeWithOpenGenericSupportString()))),
                                    Syntax.Token(SyntaxKind.CommaToken),
                                    Syntax.BinaryExpression(
                                        SyntaxKind.AssignExpression,
                                        Syntax.IdentifierName("Request"),
                                        Syntax.TypeOfExpression(Syntax.IdentifierName(useCaseNode.Request.TypeWithOpenGenericSupportString()))))));

            ProcessUsings(useCaseNode);

            if (useCaseNode.ChildNodes != null && useCaseNode.ChildNodes.Length > 0)
            {
                useCaseNodeSyntax =
                    useCaseNodeSyntax.WithInitializer(
                        Syntax.InitializerExpression(
                            SyntaxKind.ObjectInitializerExpression,
                            useCaseNodeSyntax.Initializer.Expressions.Add(GetChildsNodesSyntax(useCaseNode.ChildNodes))));
            }

            if (useCaseNode.Level == 0)
            {   // это root node для usecase
                useCaseNodeSyntax = useCaseNodeSyntax.NormalizeWhitespace(elasticTrivia: true).Format(FormattingOptions.GetDefaultOptions()).GetFormattedRoot() as ObjectCreationExpressionSyntax;
            }

            return useCaseNodeSyntax;
        }

        private void ProcessUsings(UseCaseNode useCaseNode)
        {
            var processingTypes = new List<ITypeSymbol>
                {
                    useCaseNode.ContainingClass,
                    useCaseNode.Request
                };

            ITypeSymbol[] genericTypes = null;
            if (useCaseNode.ContainingClass.TryGetGenericTypeArgsForType(out genericTypes))
            {
                processingTypes.AddRange(genericTypes);
            }

            if (useCaseNode.Request.TryGetGenericTypeArgsForType(out genericTypes))
            {
                processingTypes.AddRange(genericTypes);
            }

            foreach (var type in processingTypes)
            {
                var typeNamespace = type.ContainingNamespace.ToDisplayString(
                        new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces));
                if (!_targetUsingList.ContainsKey(typeNamespace))
                {
                    _targetUsingList.Add(typeNamespace, Syntax.UsingDirective(Syntax.IdentifierName(typeNamespace)));
                }
            }
        }

        private ExpressionSyntax GetChildsNodesSyntax(UseCaseNode[] childNodes)
        {
            var useCaseNodeListElements = new List<ExpressionSyntax>();
            var useCaseNodeListElementsSeparators = new List<SyntaxToken>();

            for (int i = 0; i < childNodes.Length - 1; i++)
            {
                useCaseNodeListElements.Add(GetUseCaseNodeSyntax(childNodes[i]));
                useCaseNodeListElementsSeparators.Add(Syntax.Token(SyntaxKind.CommaToken));
            }

            if (childNodes.Length > 0)
            {
                useCaseNodeListElements.Add(GetUseCaseNodeSyntax(childNodes[childNodes.Length - 1]));
            }

            var expressionsList = Syntax.SeparatedList(useCaseNodeListElements, useCaseNodeListElementsSeparators);

            return Syntax.BinaryExpression(
                        SyntaxKind.AssignExpression,
                        Syntax.IdentifierName("ChildNodes"),
                        Syntax.ImplicitArrayCreationExpression(Syntax.InitializerExpression(SyntaxKind.ObjectInitializerExpression, expressionsList)));
        }

        #endregion
    }
}
