using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    internal interface IEntityPartConverterHost<TEntityInstance, TPropertyInstance>
        where TEntityInstance : class, IDynamicEntityInstance
        where TPropertyInstance : class, IDynamicEntityPropertyInstance
    {
        IEntity Convert(TEntityInstance entityInstance, ICollection<TPropertyInstance> propertyInstances);
        DynamicEntityInstanceDto<TEntityInstance, TPropertyInstance> ConvertBack(IEntity entity);
    }
}