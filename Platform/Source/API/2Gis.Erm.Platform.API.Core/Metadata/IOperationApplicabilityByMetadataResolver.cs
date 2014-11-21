using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability;

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
