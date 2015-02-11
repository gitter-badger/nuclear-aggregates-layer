using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Operations.Generic.ChangeActivityStatus;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.Operations.Generic.ChangeActivityStatus
{
    public class ChangeActivityStatusAppointmentService : IChangeGenericEntityActivityStatusService<Appointment>
    {
        private readonly IUserContext _userContext;

        private readonly ICommonLog _logger;

        public ChangeActivityStatusAppointmentService(
            IPublicService publicService,
            IFinder finder,
            IAccountRepository accountRepository,
            ISecurityServiceEntityAccess entityAccessService,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IOperationScopeFactory scopeFactory,
            IUserContext userContext,
            ICommonLog logger)
        {
            this._userContext = userContext;
            this._logger = logger;
        }

        public virtual void ChangeStatus(long entityId, ActivityStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
