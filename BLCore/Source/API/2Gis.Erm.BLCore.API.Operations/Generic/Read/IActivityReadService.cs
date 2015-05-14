using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Read
{
    // FIXME {s.pomadin, 17.09.2014}: does it make sense to intro a special service contract instead of direct IOperation usage
    public interface IActivityReadService : IOperation<CheckRelatedActivitiesIdentity>
    {
        bool CheckIfActivityExistsRegarding(IEntityType entityName, long clientId);
        bool CheckIfOpenActivityExistsRegarding(IEntityType entityName, long clientId);
        IEnumerable<IEntity> LookupActivitiesRegarding(IEntityType entityName, long clientId);
    }
}