using System.Collections.Generic;

using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.Platform.Migration.Runner
{
    public class MigrationRunResult
    {
        public MigrationRunResult()
        {
            SuccessfullMigrations = new List<long>(32);
        }

        public List<long> SuccessfullMigrations { get; private set; }
        public MigrationDescriptor FailureMigration { get; set; }

        public bool IsEmpty()
        {
            return SuccessfullMigrations.Count == 0 && FailureMigration == null;
        }
    }
}
