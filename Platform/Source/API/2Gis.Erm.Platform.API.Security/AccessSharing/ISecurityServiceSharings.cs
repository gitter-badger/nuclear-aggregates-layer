using System.Collections.Generic;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.API.Security.AccessSharing
{
    public interface ISecurityServiceSharings
    {
        IEnumerable<SharingDescriptor> GetAccessSharingsForEntity(IEntityType entityName, long entityId);
        void UpdateAccessSharings(IEntityType entityName, long entityId, long entityOwnerCode, IEnumerable<SharingDescriptor> accessSharings, long userCode);
    }
}
