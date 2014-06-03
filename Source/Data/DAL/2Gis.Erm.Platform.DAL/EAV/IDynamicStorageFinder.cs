using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public interface IDynamicStorageFinder
    {
        IEnumerable<IEntity> Find<TEntityInstance, TPropertyInstance>(SpecsBundle<TEntityInstance, TPropertyInstance> specs)
            where TEntityInstance : class, IDynamicEntityInstance
            where TPropertyInstance : class, IDynamicEntityPropertyInstance;

    }
}