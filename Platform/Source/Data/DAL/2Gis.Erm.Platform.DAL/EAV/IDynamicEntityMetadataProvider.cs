using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public interface IDynamicEntityMetadataProvider
    {
        Type DetermineObjectType<TPropertyInstace>(IEnumerable<TPropertyInstace> dynamicEntityPropertyInstances)
            where TPropertyInstace : class, IDynamicEntityPropertyInstance;

        SpecsBundle<TEntityInstance, TEntityPropertyInstance> GetSpecifications<TEntityInstance, TEntityPropertyInstance>(Type entityType, IEnumerable<long> entityIds)
            where TEntityInstance : class, IDynamicEntityInstance
            where TEntityPropertyInstance : class, IDynamicEntityPropertyInstance;
    }
}