using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting
{
    public interface IReplicationCodeConverter : IInvariantSafeCrosscuttingService
    {
        long ConvertToEntityId(EntityName entityName, Guid replicationCode);
        Guid ConvertToReplicationCode(EntityName entityName, long entityId);
        IEnumerable<long> ConvertToEntityIds(EntityName entityName, IEnumerable<Guid> replicationCodes);
        IEnumerable<Guid> ConvertToReplicationCodes(EntityName entityName, IEnumerable<long> entityIds);
    }
}