using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.DI.Zones
{
    public class ZonePartInfo
    {
        public Type MarkerType { get; set; }
        public Type ZoneScope { get; set; }
        public Type BusinessModel { get; set; }
        public IEnumerable<Type> ContainedTypes { get; set; }
    }
}