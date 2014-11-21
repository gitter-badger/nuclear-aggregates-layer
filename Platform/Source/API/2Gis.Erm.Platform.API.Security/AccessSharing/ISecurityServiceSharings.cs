using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.API.Security.AccessSharing
{
    public interface ISecurityServiceSharings
    {
        IEnumerable<SharingDescriptor> GetAccessSharingsForEntity(EntityName entityName, long entityId);
        void UpdateAccessSharings(EntityName entityName, long entityId, long entityOwnerCode, IEnumerable<SharingDescriptor> accessSharings, long userCode);
    }
}
