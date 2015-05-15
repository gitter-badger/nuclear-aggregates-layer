using System.ServiceModel.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Special.FirmWorkStatus;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BLCore.Operations.Special.FirmWorkStatus
{
    public class GetFirmWorkStatusOperationService : IGetFirmWorkStatusOperationService
    {
        private readonly IFirmReadModel _firmReadModel;
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceEntityAccess _securityServiceEntityAccess;
        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;

        public GetFirmWorkStatusOperationService(IFirmReadModel firmReadModel,
                                                 IUserContext userContext,
                                                 ISecurityServiceEntityAccess securityServiceEntityAccess,
                                                 ISecurityServiceUserIdentifier securityServiceUserIdentifier)
        {
            _firmReadModel = firmReadModel;
            _userContext = userContext;
            _securityServiceEntityAccess = securityServiceEntityAccess;
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
        }

        public API.Operations.Special.FirmWorkStatus.FirmWorkStatus GetFirmWorkStatus(long firmId)
        {
            long ownerCode;
            if (!_firmReadModel.TryGetFirmOwnerCodeUnsecure(firmId, out ownerCode))
            {
                throw new EntityNotFoundException(typeof(Firm), firmId);
            }

            if (!_securityServiceEntityAccess.HasEntityAccess(EntityAccessTypes.Read,
                                                              EntityName.Firm,
                                                              _userContext.Identity.Code,
                                                              firmId,
                                                              ownerCode,
                                                              null))
            {
                throw new SecurityAccessDeniedException(string.Format("User {0} has no privilege to read firm with code {1}", _userContext.Identity.Account, firmId));
            }

            return new API.Operations.Special.FirmWorkStatus.FirmWorkStatus
                       {
                           IsInReserve = ownerCode == _securityServiceUserIdentifier.GetReserveUserIdentity().Code
                       };
        }
    }
}