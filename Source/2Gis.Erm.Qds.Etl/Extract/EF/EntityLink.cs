using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public class EntityLink
    {
        public Type EntityType { get; set; }
        public ICollection<long> Ids { get; set; }
    }
}