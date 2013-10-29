using System;

namespace DoubleGis.Erm.Platform.Migration.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class MigrationAttribute : Attribute
    {
        public MigrationAttribute(long version, string description = "")
        {
            Version = version;
            Description = description;
        }

        public long Version { get; private set; }
        public string Description { get; private set; }
    }
}