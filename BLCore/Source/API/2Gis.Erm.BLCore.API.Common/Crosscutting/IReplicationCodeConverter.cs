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
        [Obsolete("Логика теперь требует, чтобы каждому replicationCode соответствовал свой entityName, это позволит нам в одном запросе запрашивать объекты разных EntityName")]
        IEnumerable<long> ConvertToEntityIds(EntityName entityName, IEnumerable<Guid> replicationCodes);
        IEnumerable<ErmEntityInfo> ConvertToEntityIds(IEnumerable<CrmEntityInfo> crmEntities);
        IEnumerable<Guid> ConvertToReplicationCodes(EntityName entityName, IEnumerable<long> entityIds);
    }
}