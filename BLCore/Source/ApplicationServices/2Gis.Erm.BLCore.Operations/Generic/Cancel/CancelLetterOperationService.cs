using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Cancel;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Cancel
{
    public class CancelLetterOperationService : ICancelGenericOperationService<Letter>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;

        private readonly IActionLogger _actionLogger;

        private readonly ILetterReadModel _letterReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;

        private readonly ICancelLetterAggregateService _cancelLetterAggregateService;

        public CancelLetterOperationService(
            IOperationScopeFactory operationScopeFactory,
            IActionLogger actionLogger,
            ILetterReadModel letterReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            ICancelLetterAggregateService cancelLetterAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _actionLogger = actionLogger;
            _letterReadModel = letterReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _cancelLetterAggregateService = cancelLetterAggregateService;
        }

        public virtual void Cancel(long entityId)
        {           
            using (var scope = _operationScopeFactory.CreateSpecificFor<CancelIdentity, Letter>())
            {
                var letter = _letterReadModel.GetLetter(entityId);
                var originalStatus = letter.Status;

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, letter.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", letter.Header, BLResources.SecurityAccessDenied));
                }       

                _cancelLetterAggregateService.Cancel(letter);

                _actionLogger.LogChanges(letter, x => x.Status, originalStatus, ActivityStatus.Canceled);

                scope.Updated<Letter>(entityId);
                scope.Complete();
            }
        }
    }
}
