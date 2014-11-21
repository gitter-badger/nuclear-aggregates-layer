using System.Linq;

using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.API.Security
{
    public interface ISecurityServiceEntityAccessInternal : ISecurityServiceEntityAccess, IInvariantSafeCrosscuttingService
    {
        IQueryable RestrictQuery(IQueryable query, EntityName entityName, long userCode);

        EntityAccessTypes GetCommonEntityAccessForMetadata(EntityName entityName, long userCode);
    }
}
