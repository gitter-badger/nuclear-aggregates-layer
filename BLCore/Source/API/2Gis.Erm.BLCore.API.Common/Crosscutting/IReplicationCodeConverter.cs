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
        [Obsolete("Логика теперь требует, чтобы каждому replicationCode соответствовал свой entityName, это позволит нам в одном запросе запрашивать объекты разных EntityName")]
        IEnumerable<long> ConvertToEntityIds(IEntityType entityName, IEnumerable<Guid> replicationCodes);
        IEnumerable<ErmEntityInfo> ConvertToEntityIds(IEnumerable<CrmEntityInfo> crmEntities);
        IEnumerable<Guid> ConvertToReplicationCodes(IEntityType entityName, IEnumerable<long> entityIds);
    }
}