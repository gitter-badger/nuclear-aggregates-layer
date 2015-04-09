using System;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Complete;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Complete;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Complete;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Complete
{
    public class CompleteLetterOperationService : ICompleteGenericOperationService<Letter>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ILetterReadModel _letterReadModel;

        private readonly IActionLogger _actionLogger;

        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly ICompleteLetterAggregateService _completeLetterAggregateService;

        public CompleteLetterOperationService(
            IOperationScopeFactory operationScopeFactory,
            ILetterReadModel letterReadModel,
            IActionLogger actionLogger,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            ICompleteLetterAggregateService completeLetterAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _letterReadModel = letterReadModel;
            _actionLogger = actionLogger;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _completeLetterAggregateService = completeLetterAggregateService;            
        }

        public virtual void Complete(long entityId)
        {           
            using (var scope = _operationScopeFactory.CreateSpecificFor<CompleteIdentity, Letter>())
            {
                var letter = _letterReadModel.GetLetter(entityId);
                var originalStatus = letter.Status;
                var userLocale = _userContext.Profile.UserLocaleInfo;

                if (userLocale.UserTimeZoneInfo.ConvertDateFromUtc(letter.ScheduledOn) > userLocale.UserTimeZoneInfo.ConvertDateFromLocal(DateTime.Now))
                {
                    throw new BusinessLogicException(BLResources.ActivityClosingInFuturePeriodDenied);
                }

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, letter.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", letter.Header, BLResources.SecurityAccessDenied));
                }       

                _completeLetterAggregateService.Complete(letter);

                _actionLogger.LogChanges(letter, x => x.Status, originalStatus, ActivityStatus.Completed);

                scope.Updated<Letter>(entityId);
                scope.Complete();
            }
        }
    }
}
