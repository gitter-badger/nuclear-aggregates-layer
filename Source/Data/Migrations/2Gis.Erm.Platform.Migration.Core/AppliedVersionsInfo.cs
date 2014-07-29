using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.Migration.Core
{
    public sealed class AppliedVersionsInfo
    {
		private readonly HashSet<long> _versionsApplied = new HashSet<long>();

        public long Latest()
        {
            return _versionsApplied.DefaultIfEmpty().Max();
        }

        public void AddAppliedMigration(long migration)
        {
            _versionsApplied.Add(migration);
        }

        public bool HasAppliedMigration(long migration)
        {
            return _versionsApplied.Contains(migration);
        }

        public IEnumerable<long> GetAppliedMigrations()
        {
            return _versionsApplied.OrderBy(x => x);
        }
    }
}
