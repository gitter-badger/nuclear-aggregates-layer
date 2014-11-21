using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.Migration.Core
{
    public interface IMigrationDescriptorsProvider
    {
        List<MigrationDescriptor> MigrationDescriptors { get; }
    }
}
