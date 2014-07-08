using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices
{
    public interface IReplicationPersistenceService : ISimplifiedPersistenceService
    {
        void ReplicateToMscrm(Type entityType, IEnumerable<long> ids, int timeout, out IEnumerable<long> notReplicated);
    }
}
