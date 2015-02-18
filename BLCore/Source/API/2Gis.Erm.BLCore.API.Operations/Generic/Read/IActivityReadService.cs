using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Read
{
    // FIXME {s.pomadin, 17.09.2014}: does it make sense to intro a special service contract instead of direct IOperation usage
    public interface IActivityReadService : IOperation<CheckRelatedActivitiesIdentity>
    {
        bool CheckIfActivityExistsRegarding(EntityName entityName, long clientId);

        bool CheckIfOpenActivityExistsRegarding(EntityName entityName, long clientId);
        
        IEnumerable<IEntity> LookupActivitiesRegarding(EntityName entityName, long clientId);

        void CheckIfAnyEntityReferencesContainsReserve(IEnumerable<EntityReference> references);

        void CheckIfEntityReferencesContainsReserve(EntityReference reference);
    }
}