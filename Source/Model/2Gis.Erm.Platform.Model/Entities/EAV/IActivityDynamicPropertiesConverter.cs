using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.Model.Entities.EAV
{
    // FIXME {a.rechkalov;d.ivanov, 05.02.2014}: Заменить все использованния этого интерфейса на IDynamicEntityPropertiesConverter<TEntity, TInstance, TPropertyInstace>
    [Obsolete("Use strongly typed IDynamicEntityPropertiesConverter<TEntity, TInstance, TPropertyInstace>")]
    public interface IActivityDynamicPropertiesConverter
    {
        TActivity ConvertFromActivityInstance<TActivity>(ActivityInstance activityInstance, ICollection<ActivityPropertyInstance> propertyInstances) 
            where TActivity : ActivityBase, new();

        Tuple<ActivityInstance, ICollection<ActivityPropertyInstance>> ConvertToActivityInstance<TActivity>(TActivity activity,
                                                                                                            ICollection<ActivityPropertyInstance> activityPropertyInstances)
            where TActivity : ActivityBase;
    }
}