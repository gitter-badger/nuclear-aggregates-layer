using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Read
{
    // FIXME {s.pomadin, 17.09.2014}: does it make sense to intro a special service contract instead of direct IOperation usage
    public interface IActivityReadService : IOperation<CheckRelatedActivitiesIdentity>
    {
        bool CheckIfRelatedActivitiesExists(EntityName entityName, long clientId);
        
        bool CheckIfRelatedActiveActivitiesExists(EntityName entityName, long clientId);
    }
}