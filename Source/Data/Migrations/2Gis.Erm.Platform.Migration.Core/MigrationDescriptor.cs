using System;

namespace DoubleGis.Erm.Platform.Migration.Core
{
    public class MigrationDescriptor
    {
        public Type Type { get; set; }
        public long Version { get; set; }
        public string Description { get; set; }
    }
}