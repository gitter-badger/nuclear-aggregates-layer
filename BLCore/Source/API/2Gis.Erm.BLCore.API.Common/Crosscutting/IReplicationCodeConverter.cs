using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Crosscutting;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting
{
    public interface IReplicationCodeConverter : IInvariantSafeCrosscuttingService
    {
        long ConvertToEntityId(IEntityType entityName, Guid replicationCode);
        Guid ConvertToReplicationCode(IEntityType entityName, long entityId);
        IEnumerable<long> ConvertToEntityIds(IEntityType entityName, IEnumerable<Guid> replicationCodes);
        IEnumerable<Guid> ConvertToReplicationCodes(IEntityType entityName, IEnumerable<long> entityIds);
    }
}