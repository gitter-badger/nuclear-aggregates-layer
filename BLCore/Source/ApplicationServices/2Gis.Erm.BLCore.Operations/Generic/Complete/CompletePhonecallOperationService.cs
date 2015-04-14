using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Complete;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Complete;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Complete;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Complete
{
    public class CompletePhonecallOperationService : ICompleteGenericOperationService<Phonecall>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;

        private readonly IActionLogger _actionLogger;

        private readonly IUserContext _userContext;
        private readonly IChangeDealStageOperationService _changeDealStageOperationService;

        private readonly ICompletePhonecallAggregateService _completePhonecallAggregateService;

        public CompletePhonecallOperationService(
            IOperationScopeFactory operationScopeFactory,
            IPhonecallReadModel phonecallReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IActionLogger actionLogger,
            IUserContext userContext,
            IChangeDealStageOperationService changeDealStageOperationService,
            ICompletePhonecallAggregateService completePhonecallAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _phonecallReadModel = phonecallReadModel;
            _entityAccessService = entityAccessService;
            _actionLogger = actionLogger;
            _userContext = userContext;
            _changeDealStageOperationService = changeDealStageOperationService;
            _completePhonecallAggregateService = completePhonecallAggregateService;
        }

        public void Complete(long entityId)
        {            
            using (var scope = _operationScopeFactory.CreateSpecificFor<CompleteIdentity, Phonecall>())
            {
                var phonecall = _phonecallReadModel.GetPhonecall(entityId);
                var originalStatus = phonecall.Status;

                var userLocale = _userContext.Profile.UserLocaleInfo;

                if (userLocale.UserTimeZoneInfo.ConvertDateFromUtc(phonecall.ScheduledOn).Date > userLocale.UserTimeZoneInfo.ConvertDateFromLocal(DateTime.Now).Date)
                {
                    throw new BusinessLogicException(BLResources.ActivityClosingInFuturePeriodDenied);
                }

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, phonecall.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", phonecall.Header, BLResources.SecurityAccessDenied));
                } 

                _completePhonecallAggregateService.Complete(phonecall);

                var phonecallRegardingObjects = _phonecallReadModel.GetRegardingObjects(entityId);
                UpdateDealStage(phonecallRegardingObjects, phonecall);

                _actionLogger.LogChanges(phonecall, x => x.Status, originalStatus, ActivityStatus.Completed);

                scope.Updated<Phonecall>(entityId);
                scope.Complete();
            }
        }      

        private static DealStage ConvertToStage(PhonecallPurpose purpose)
        {
            switch (purpose)
            {
                case PhonecallPurpose.FirstCall:
                    return DealStage.CollectInformation;

                case PhonecallPurpose.ProductPresentation:
                case PhonecallPurpose.OpportunitiesPresentation:
                    return DealStage.HoldingProductPresentation;

                case PhonecallPurpose.OfferApproval:
                case PhonecallPurpose.DecisionApproval:
                    return DealStage.MatchAndSendProposition;

                default:
                    return DealStage.None;
            }
        }

        /// <summary>
        /// Tries to update the related deal stage if any.
        /// </summary>
        /// <remarks>
        /// See the specs on https://confluence.2gis.ru/pages/viewpage.action?pageId=48464616.
        /// </remarks>
        private void UpdateDealStage(IEnumerable<PhonecallRegardingObject> regardingObjects, Phonecall phonecall)
        {
            var dealRef = regardingObjects.FirstOrDefault(x => x.TargetEntityName == EntityName.Deal);
            if (dealRef == null)
            {
                return;
            }

            var dealId = dealRef.TargetEntityId;
            var purpose = phonecall.Purpose;

            var newDealStage = ConvertToStage(purpose);
            if (newDealStage == DealStage.None)
            {
                return;
            }

            _changeDealStageOperationService.Change(dealId, newDealStage);
        }
    }
}
