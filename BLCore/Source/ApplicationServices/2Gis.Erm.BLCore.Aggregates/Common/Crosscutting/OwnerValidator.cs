using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting
{
    public sealed class OwnerValidator : IOwnerValidator
    {
        private readonly ISecurityServiceUserIdentifier _securityService;

        public OwnerValidator(ISecurityServiceUserIdentifier securityService)
        {
            _securityService = securityService;
        }

        public void CheckIsNotReserve(ICuratedEntity entity)
        {
            var reserveUser = _securityService.GetReserveUserIdentity();
            if (entity.OwnerCode == reserveUser.Code)
            {
                throw new EntityIsInReserveException(string.Format(BLResources.PleaseUseQualifyOperation, reserveUser.DisplayName));
            }
        }
    }
}