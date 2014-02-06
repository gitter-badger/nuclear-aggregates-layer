using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.DTO
{
    public class DictionaryEntityInstanceDto
    {
        public DictionaryEntityInstance DictionaryEntityInstance { get; set; }
        public ICollection<DictionaryEntityPropertyInstance> DictionaryEntityPropertyInstances { get; set; }
    }
}