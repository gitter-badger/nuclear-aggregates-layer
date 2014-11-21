using System.Linq;
using System.Text;

using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Utils
{
    public static class TypeUtils
    {
        private static System.Reflection.AssemblyName GetAssemblyName(ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.Kind)
            {
                case CommonSymbolKind.ArrayType:
                {
                    var elementType = ((ArrayTypeSymbol)typeSymbol).ElementType;
                    return GetAssemblyName(elementType);
                }
                case CommonSymbolKind.TypeParameter:
                {
                    var typeParameterType = (TypeParameterSymbol)typeSymbol;
                    TypeSymbol containingType = typeParameterType.DeclaringType ?? typeParameterType.DeclaringMethod.ContainingType;
                    return GetAssemblyName(containingType);
                }

                    // May need similar special handling for other kinds of TypeSymbols.
                default:
                    return typeSymbol.ContainingAssembly.Identity.ToAssemblyName();
            }
        }

        public static string TypeDescritionString(this INamedTypeSymbol typeSymbol)
        {
            // May need to tweak the SymbolDisplayFormat to make it work for all cases
            // For example, more work will be required for generic types...
            var targetType =
                typeSymbol.ToDisplayString(
                    new SymbolDisplayFormat(
                        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.None));

            return targetType + ", " + GetAssemblyName(typeSymbol).FullName;

        }

        public static string TypeWithOpenGenericSupportString(this INamedTypeSymbol typeSymbol)
        {
            var basePart =
                typeSymbol.ToDisplayString(
                    new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes));
            if (!typeSymbol.IsGenericType)
            {
                return basePart;
            }

            var sb = new StringBuilder();
            sb.Append("<");

            for (int i = 0; i < typeSymbol.TypeArguments.Count - 1; i++)
            {
                var tArg = typeSymbol.TypeArguments[i];

                if (tArg.BaseType != null)
                {   // аргумент - уже конкретный тип,  а не просто параметр generic
                    sb.Append(tArg.Name);
                }

                sb.Append(",");
            }

            if (typeSymbol.TypeArguments.Count > 0)
            {   // последний параметр
                var tArg = typeSymbol.TypeArguments[typeSymbol.TypeArguments.Count - 1];

                if (tArg.BaseType != null)
                {   // аргумент - уже конкретный тип,  а не просто параметр generic
                    sb.Append(tArg.Name);
                }
            }

            sb.Append(">");
            return basePart + sb;
        }

        public static bool TryGetGenericTypeArgsForType(this INamedTypeSymbol namedTypeSymbol, out ITypeSymbol[] genericArgs)
        {
            genericArgs = null;

            if (!namedTypeSymbol.IsGenericType)
            {
                return false;
            }

            genericArgs = namedTypeSymbol.TypeArguments.Where(t => t.BaseType != null).ToArray();
            return genericArgs.Length > 0;
        }
    }    
}
