using System;
using System.Collections.Generic;
using System.Linq;

using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindPublicServiceUseCases
{
    class InvocationsProcessor
    {
        private readonly InvocationProcessorConfig _config;

        public class InvocationProcessorConfig
        {
            public ISolution Solution { get; set; }
            public ISemanticModel SemanticModel { get; set; }
            public IDocument Document { get; set; }
            public CommonSyntaxNode ContainingElement { get; set; }
            public INamedTypeSymbol ContainingClass { get; set; }
            public Action<UseCaseEndpointDescriptor, bool> AddInvocation { get; set; }
            public ISymbol TargetMethodContainingSymbol { get; set; }
            public string TargetMethodName { get; set; }
            public ISymbol RequestBaseSymbol { get; set; }
        }

        public InvocationsProcessor(InvocationProcessorConfig config)
        {
            _config = config;
        }

        public void ProcessInovacations()
        {
            var inovacations = _config.ContainingElement.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var invocation in inovacations)
            {
                IMethodSymbol callSymbol = null;
                if (!TryGetTargetMethodCall(_config.SemanticModel, invocation, out callSymbol))
                {
                    continue;
                }

                var containingClassMember = ResolveContainingMember(invocation);
                if (containingClassMember == null)
                {
                    throw new InvalidOperationException("Unsupported containing member processing");
                }

                var requestSyntax = invocation.ArgumentList.Arguments[0].DescendantNodes().OfType<NameSyntax>().First();
                var requestSymbol = _config.SemanticModel.GetSymbolInfo(requestSyntax).Symbol;
                if (requestSymbol == null)
                {
                    throw new InvalidOperationException("Can't find request symbol");
                }

                INamedTypeSymbol requestTypeSymbol = ResolveSymbol(requestSymbol);
                if (requestTypeSymbol == null)
                {
                    throw new InvalidOperationException("Not supported type of request instance provider. Base symbol: " + requestSymbol);
                }

                bool isValidRequest = false;
                if (requestTypeSymbol.Equals(_config.RequestBaseSymbol))
                {
                    var methodSymbol = containingClassMember as IMethodSymbol;
                    if (methodSymbol == null)
                    {
                        return;
                    }

                    var invocationsProcesssor = new InvocationsProcessor(
                        new InvocationProcessorConfig
                        {
                            Solution = _config.Solution,
                            Document = _config.Document,
                            SemanticModel = _config.SemanticModel,
                            AddInvocation = _config.AddInvocation,
                            ContainingClass = _config.ContainingClass,
                            ContainingElement = _config.ContainingClass.DeclaringSyntaxNodes[0],
                            TargetMethodContainingSymbol = _config.ContainingClass, 
                            TargetMethodName = methodSymbol.Name,
                            RequestBaseSymbol = _config.RequestBaseSymbol
                        });
                    invocationsProcesssor.ProcessInovacations();
                    continue;
                }

                for (var baseType = requestTypeSymbol.BaseType; baseType != null; baseType = baseType.BaseType)
                {
                    if (baseType.OriginalDefinition.Equals(_config.RequestBaseSymbol))
                    {
                        isValidRequest = true;
                        break;
                    }
                }

                var useCaseDescriptor = new UseCaseEndpointDescriptor
                {
                    Description = "PublicService_Explicitly",
                    DeclaringDocument = _config.Document.Id,
                    ContainingClass = _config.ContainingClass,
                    CallSyntaxTree = invocation,
                    ContainingClassMember = containingClassMember,
                    RequestType = requestTypeSymbol
                };

                _config.AddInvocation(useCaseDescriptor, isValidRequest);
            }
        }

        public static readonly IEnumerable<SyntaxKind> SupportedContainingMembers = new[]
            {
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.MethodDeclaration,
                SyntaxKind.FieldDeclaration,
                SyntaxKind.PropertyDeclaration
            };
        private ISymbol ResolveContainingMember(InvocationExpressionSyntax invocation)
        {
            var methodDeclaration = invocation.Ancestors().OfType<BaseMethodDeclarationSyntax>().FirstOrDefault();
            if (methodDeclaration != null)
            {
                return _config.SemanticModel.GetDeclaredSymbol(methodDeclaration);
            }

            var fieldDeclaration = invocation.Ancestors().OfType<BaseFieldDeclarationSyntax>().FirstOrDefault();
            if (fieldDeclaration != null)
            {
                var field = (FieldDeclarationSyntax)fieldDeclaration;
                return _config.SemanticModel.GetDeclaredSymbol(field.Declaration.Variables.First());
            }

            var propertyDeclaration = invocation.Ancestors().OfType<BasePropertyDeclarationSyntax>().FirstOrDefault();
            if (propertyDeclaration != null)
            {
                return _config.SemanticModel.GetDeclaredSymbol(propertyDeclaration);
            }

            return null;
        }

        private bool TryGetTargetMethodCall(ISemanticModel semanticModel, InvocationExpressionSyntax invocation, out IMethodSymbol methodSymbol)
        {
            methodSymbol = null;

            if (invocation == null || !invocation.ArgumentList.Arguments.Any())
            {
                return false;
            }

            var callSymbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
            if (callSymbol == null || string.CompareOrdinal(callSymbol.Name, _config.TargetMethodName) != 0)
            {
                return false;
            }

            if (callSymbol.ConstructedFrom == null 
                || !callSymbol.ConstructedFrom.ContainingType.Equals(_config.TargetMethodContainingSymbol))
            {
                return false;
            }

            return true;
        }

        private INamedTypeSymbol ResolveSymbol(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case CommonSymbolKind.Method:
                    {
                        // request создается на new, либо возвращается из вызова метода (фабричного и т.п.)
                        var requestPrepareMethodSymbol = symbol as MethodSymbol;
                        if (requestPrepareMethodSymbol == null)
                        {
                            break;
                        }

                        if (requestPrepareMethodSymbol.MethodKind == MethodKind.Constructor)
                        {
                            return requestPrepareMethodSymbol.ContainingType;
                        }

                        var requestFromMethodType = requestPrepareMethodSymbol.ReturnType as INamedTypeSymbol;
                        if (requestFromMethodType != null)
                        {
                            return requestPrepareMethodSymbol.ContainingType;
                        }
                        break;
                    }
                case CommonSymbolKind.Local:
                    {
                        var requestContainingLocalSymbol = symbol as ILocalSymbol;
                        if (requestContainingLocalSymbol == null)
                        {
                            break;
                        }

                        return requestContainingLocalSymbol.Type as INamedTypeSymbol;
                    }
                case CommonSymbolKind.Parameter:
                    {
                        var requestContainingParameterSymbol = symbol as IParameterSymbol;
                        if (requestContainingParameterSymbol == null)
                        {
                            break;
                        }

                        return requestContainingParameterSymbol.Type as INamedTypeSymbol;
                    }
            }

            return null;
        }
    }
}
