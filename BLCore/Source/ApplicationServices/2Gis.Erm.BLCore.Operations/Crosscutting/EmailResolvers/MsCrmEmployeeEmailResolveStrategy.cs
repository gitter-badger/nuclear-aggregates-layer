using System;

using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting.EmailResolvers
{
    public class MsCrmEmployeeEmailResolveStrategy : IEmployeeEmailResolveStrategy
    {
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly ISecurityServiceUserIdentifier _securityService;
        private readonly ICommonLog _logger;

        public MsCrmEmployeeEmailResolveStrategy(IMsCrmSettings msCrmSettings, ISecurityServiceUserIdentifier securityService, ICommonLog logger)
        {
            _msCrmSettings = msCrmSettings;
            _securityService = securityService;
            _logger = logger;
        }

        #region Implementation of IEmployeeEmailResolveStrategy

        public bool TryResolveEmail(long employeeUserCode, out string email)
        {
            email = null;

            try
            {
                var userInfo = _securityService.GetUserInfo(employeeUserCode);
                if (userInfo == null)
                {
                    _logger.Error("Can't find user info for specified user code: " + employeeUserCode);
                    return false;
                }

                var crmDataContext = _msCrmSettings.CreateDataContext();

                var accountOwnerUserInfo = crmDataContext.GetSystemUserByDomainName(userInfo.Account, true);
                if (accountOwnerUserInfo != null && !string.IsNullOrEmpty(accountOwnerUserInfo.InternalEmailAddress))
                {
                    email = accountOwnerUserInfo.InternalEmailAddress;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Can't get email info from MSCRM for user code: " + employeeUserCode);
            }

            return false;
        }

        #endregion
    }
}