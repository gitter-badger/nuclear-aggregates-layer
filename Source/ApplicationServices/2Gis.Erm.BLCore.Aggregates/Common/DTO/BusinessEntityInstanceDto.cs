using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.DTO
{
    public class BusinessEntityInstanceDto
    {
        public BusinessEntityInstance EntityInstance { get; set; }
        public ICollection<BusinessEntityPropertyInstance> PropertyInstances { get; set; }
    }
}