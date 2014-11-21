using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.Platform.API.Security
{
    public interface ISecurityServiceFunctionalAccess : IInvariantSafeCrosscuttingService
    {
        bool HasFunctionalPrivilegeGranted(FunctionalPrivilegeName privilege, long userCode);
        int[] GetFunctionalPrivilege(FunctionalPrivilegeName privilege, long userCode);

        // TODO: перенести в соответствующий handler
        bool HasOrderChangeDocumentsDebtAccess(long organizationUnitId, long userCode);
    }
}