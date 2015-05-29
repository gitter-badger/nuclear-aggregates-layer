using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements.Concrete.Hierarchy;

using Roslyn.Compilers.Common;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Utils
{
    public static class Workarounds
    {
        private static readonly IDictionary<ISemanticModel, HashSet<INamedTypeSymbol>> AllExcludedTypesCantGetAllInterfacesMap = new Dictionary<ISemanticModel, HashSet<INamedTypeSymbol>>();
        
        /// <summary>
        /// Список типов для которых версия roslyn 09_2012 не может получить список интерфейсов - из-за stackoverflowexception
        /// </summary>
        public static HashSet<Type> CantGetAllInterfacesTypes =
            new HashSet<Type>(
                new[]
                    {
                        typeof(HierarchyMetadata),
                        typeof(HierarchyMetadataBuilder)
                    });

        public static HashSet<INamedTypeSymbol> GetAllExcludedTypesCantGetAllInterfaces(this ISemanticModel semanticModel)
        {
            HashSet<INamedTypeSymbol> excludedTypesSet;
            if (!AllExcludedTypesCantGetAllInterfacesMap.TryGetValue(semanticModel, out excludedTypesSet))
            {
                var excludedTypes = CantGetAllInterfacesTypes.Select(t => semanticModel.Compilation.GetTypeByMetadataName(t.FullName));
                excludedTypesSet = new HashSet<INamedTypeSymbol>(excludedTypes);
                AllExcludedTypesCantGetAllInterfacesMap.Add(semanticModel, excludedTypesSet);
            }

            return excludedTypesSet;
        }

        public static HashSet<string> ExcludedDocumentNames = new HashSet<string>(new[]
            {
                "UnityViewModelActionsResolver", 
                "UnityViewModelContextualNavigationResolver"
            });

        public static HashSet<string> ExcludedProjectNames = new HashSet<string>(new[]
            {
                "2Gis.Erm.UI.WPF.Client", 
                "2Gis.Erm.ExtractUseCases",
                "2Gis.Erm.UI.WPF.Infrastructure",
                "2Gis.Erm.Model.Metadata"
            });
    }
}
