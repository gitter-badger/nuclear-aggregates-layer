using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Metadata
{
    public interface IOperationApplicabilityByMetadataResolver
    {
        /// <summary>
        /// Генерит карту доступности операций, используя:
        /// - данные соответствия операций и реализаций (entityspecific и noncoupled)
        /// - реестр metadata detail из сборки metadata
        /// </summary>
        Dictionary<int, OperationApplicability> ResolveOperationsApplicability(
            IReadOnlyDictionary<Type, Dictionary<EntitySet, Type>> entitySpecificOperations,
            IReadOnlyDictionary<Type, Type> notCoupledOperations,
            IReadOnlyDictionary<Type, IOperationIdentity> operations2IdentitiesMap);
    }
}
