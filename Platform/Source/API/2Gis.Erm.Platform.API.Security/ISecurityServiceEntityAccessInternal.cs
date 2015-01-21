using System.Linq;

using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Common.Crosscutting;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.API.Security
{
    public interface ISecurityServiceEntityAccessInternal : ISecurityServiceEntityAccess, IInvariantSafeCrosscuttingService
    {
        IQueryable RestrictQuery(IQueryable query, IEntityType entityName, long userCode);

        EntityAccessTypes GetCommonEntityAccessForMetadata(IEntityType entityName, long userCode);
    }
}
