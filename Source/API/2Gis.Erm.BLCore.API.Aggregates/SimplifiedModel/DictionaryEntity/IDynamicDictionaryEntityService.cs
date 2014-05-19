using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.DictionaryEntity
{
    public interface IDynamicDictionaryEntityService : ISimplifiedModelConsumer
    {
        long Create(DictionaryEntityInstance dictionaryEntityInstance, IEnumerable<DictionaryEntityPropertyInstance> propertyInstances);
        void Update(DictionaryEntityInstance dictionaryEntityInstance, IEnumerable<DictionaryEntityPropertyInstance> propertyInstances);
        void Delete(DictionaryEntityInstance dictionaryEntityInstance, IEnumerable<DictionaryEntityPropertyInstance> propertyInstances);
    }
}