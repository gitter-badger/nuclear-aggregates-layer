using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Utils
{
    public static class SemanticUtils
    {
        /// <summary>
        /// Gets the symbol of a node.
        /// </summary>
        /// <param name="node">The target node.</param>
        /// <param name="identifierName">An optional identifier name.</param>
        /// <returns>The associated symbol.</returns>
        public static ISymbol GetSymbolFromAnyNodeType(this ISemanticModel semanticModel, SyntaxNode node, string identifierName = null)
        {
            Symbol symbol = null;

            // find symbol of declaration
            symbol = semanticModel.GetDeclaredSymbol(node) as Symbol;

            if (symbol == null)
            {
                // find symbol of an expression
                var symbolInfo = semanticModel.GetSymbolInfo(node);
                symbol = symbolInfo.Symbol as Symbol;

                if (symbol == null)
                {
                    if (symbolInfo.CandidateSymbols.Count == 1)
                    {
                        symbol = symbolInfo.CandidateSymbols[0] as Symbol;
                    }

                    // attempt using symbol lookup
                    if (symbol == null && identifierName != null)
                    {
                        symbol = semanticModel.LookupSymbols(node.Span.Start, name: identifierName).SingleOrDefault() as Symbol;
                    }
                }
            }

            return symbol;
        }

        public static INamedTypeSymbol GetDeclaredSymbolExcludeUnsupported(this ISemanticModel semanticModel, ClassDeclarationSyntax classDeclaration)
        {
            var excludedTypes = semanticModel.GetAllExcludedTypesCantGetAllInterfaces();

            var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
            if (classSymbol == null
                // || (classSymbol.BaseType != null && classSymbol.BaseType.IsGenericType && classSymbol.BaseType.TypeArguments.Count > 1 && classSymbol.BaseType.TypeArguments.Contains(classSymbol))
                || excludedTypes.Contains(classSymbol))
            {
                return null;
            }

            return classSymbol;
        }
    }
}
