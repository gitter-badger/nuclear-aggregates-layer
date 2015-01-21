using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL.EAV
{
    public class DynamicEntityInstanceDto<T, TProperty>
        where T : IDynamicEntityInstance
        where TProperty : IDynamicEntityPropertyInstance
    {
        public T EntityInstance { get; set; }
        public ICollection<TProperty> PropertyInstances { get; set; }
    }
}