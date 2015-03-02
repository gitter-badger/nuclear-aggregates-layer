using System.Collections.Generic;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity
{
    public interface IActivityReferenceReader
    {
        IEnumerable<EntityReference> GetRegardingObjects(EntityName name, long entityId);
        IEnumerable<EntityReference> GetAttendees(EntityName entityName, long entityId);
        IEnumerable<EntityReference> FindAutoCompleteReferences(EntityReference entity);
        EntityReference FindClientContact(IEnumerable<EntityReference> clientId);
        EntityReference ToEntityReference(EntityName entityName, long? entityId);
    }
}
