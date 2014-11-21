using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices
{
    public interface IReplicationPersistenceService : ISimplifiedPersistenceService
    {
        void ReplicateToMsCrm(Type entityType, IReadOnlyCollection<long> ids, TimeSpan timeout, out IReadOnlyCollection<long> notReplicated);
    }
}
