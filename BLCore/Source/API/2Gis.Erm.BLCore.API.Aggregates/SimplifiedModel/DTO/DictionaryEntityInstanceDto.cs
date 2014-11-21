using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.DTO
{
    public class DictionaryEntityInstanceDto
    {
        public DictionaryEntityInstance DictionaryEntityInstance { get; set; }
        public ICollection<DictionaryEntityPropertyInstance> DictionaryEntityPropertyInstances { get; set; }
    }
}