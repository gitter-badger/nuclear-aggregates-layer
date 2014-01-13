using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.DTO
{
    public sealed class DeactivateLockDto
    {
        public Lock Lock { get; set; }
        public decimal LockBalance { get; set; }
        public IEnumerable<LockDetail> Details { get; set; }
    }
}