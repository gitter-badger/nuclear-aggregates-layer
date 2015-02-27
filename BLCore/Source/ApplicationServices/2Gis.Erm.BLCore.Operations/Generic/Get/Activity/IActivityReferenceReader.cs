using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get.Activity
{
    public interface IActivityReferenceReader
    {
        IEnumerable<EntityReference> GetRegardingObjects(EntityName name, long entityId);
        IEnumerable<EntityReference> GetAttendees(EntityName entityName, long entityId);

        IEnumerable<EntityReference> FindAutoCompleteReferences(EntityReference entity);
    }
}
