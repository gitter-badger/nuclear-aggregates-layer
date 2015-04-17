using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Cancel;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Cancel
{
    public class CancelPhonecallOperationService : ICancelGenericOperationService<Phonecall>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPhonecallReadModel _phonecallReadModel;

        private readonly IActionLogger _actionLogger;

        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly ICancelPhonecallAggregateService _cancelPhonecallAggregateService;

        public CancelPhonecallOperationService(
            IOperationScopeFactory operationScopeFactory,
            IPhonecallReadModel phonecallReadModel,
            IActionLogger actionLogger,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            ICancelPhonecallAggregateService cancelPhonecallAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _phonecallReadModel = phonecallReadModel;
            _actionLogger = actionLogger;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _cancelPhonecallAggregateService = cancelPhonecallAggregateService;
        }

        public void Cancel(long entityId)
        {            
            using (var scope = _operationScopeFactory.CreateSpecificFor<CancelIdentity, Phonecall>())
            {
                var phonecall = _phonecallReadModel.GetPhonecall(entityId);
                var originalStatus = phonecall.Status;

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, phonecall.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", phonecall.Header, BLResources.SecurityAccessDenied));
                } 

                _cancelPhonecallAggregateService.Cancel(phonecall);

                _actionLogger.LogChanges(phonecall, x => x.Status, originalStatus, ActivityStatus.Canceled);

                scope.Updated<Phonecall>(entityId);
                scope.Complete();
            }
        }
    }
}
