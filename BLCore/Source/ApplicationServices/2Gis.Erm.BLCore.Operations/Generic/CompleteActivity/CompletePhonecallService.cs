﻿using System.Collections.Generic;
using System.Linq;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Complete;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Complete
{
    public class CompletePhonecallService : ICompleteGenericActivityService<Phonecall>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly IChangeDealStageOperationService _changeDealStageOperationService;
        private readonly IChangePhonecallStatusAggregateService _changePhonecallStatusAggregateService;

        public CompletePhonecallService(
            IOperationScopeFactory operationScopeFactory,
            IPhonecallReadModel phonecallReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            IChangeDealStageOperationService changeDealStageOperationService,
            IChangePhonecallStatusAggregateService changePhonecallStatusAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _phonecallReadModel = phonecallReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _changeDealStageOperationService = changeDealStageOperationService;
            _changePhonecallStatusAggregateService = changePhonecallStatusAggregateService;
        }
       
        public void Complete(long entityId)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<CompleteIdentity, Phonecall>())
            {
                var phonecall = _phonecallReadModel.GetPhonecall(entityId);

                if (phonecall.Status != ActivityStatus.InProgress)
                {
                    throw new BusinessLogicException(string.Format(BLResources.CannotCompleteFinishedOrClosedActivity, phonecall.Header));
                }

                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, phonecall.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", phonecall.Header, BLResources.SecurityAccessDenied));
                } 

                _changePhonecallStatusAggregateService.Change(phonecall, ActivityStatus.Completed);

                var phonecallRegardingObjects = _phonecallReadModel.GetRegardingObjects(entityId);
                UpdateDealStage(phonecallRegardingObjects, phonecall);

                scope.Updated<Phonecall>(entityId);
                scope.Complete();
            }
        }      

        private static DealStage ConvertToStage(ActivityPurpose purpose)
        {
            switch (purpose)
            {
                case ActivityPurpose.FirstCall:
                    return DealStage.CollectInformation;

                case ActivityPurpose.ProductPresentation:
                case ActivityPurpose.OpportunitiesPresentation:
                    return DealStage.HoldingProductPresentation;

                case ActivityPurpose.OfferApproval:
                case ActivityPurpose.DecisionApproval:
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
