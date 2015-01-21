using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public interface IDynamicStorageFinder
    {
        IEnumerable<IEntity> Find<TEntityInstance, TPropertyInstance>(SpecsBundle<TEntityInstance, TPropertyInstance> specs)
            where TEntityInstance : class, IDynamicEntityInstance
            where TPropertyInstance : class, IDynamicEntityPropertyInstance;
    }
}