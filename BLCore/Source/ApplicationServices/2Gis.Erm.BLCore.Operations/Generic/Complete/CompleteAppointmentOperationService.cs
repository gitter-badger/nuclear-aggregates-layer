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
    public class CompleteAppointmentOperationService : ICompleteGenericOperationService<Appointment>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly IChangeDealStageOperationService _changeDealStageOperationService;
        private readonly ICompleteAppointmentAggregateService _completeAppointmentAggregateService;        

        public CompleteAppointmentOperationService(
            IOperationScopeFactory operationScopeFactory, 
            IAppointmentReadModel appointmentReadModel,
            ISecurityServiceEntityAccess entityAccessService,
            IUserContext userContext,
            IChangeDealStageOperationService changeDealStageOperationService,
            ICompleteAppointmentAggregateService completeAppointmentAggregateService)
        {
            _operationScopeFactory = operationScopeFactory;
            _appointmentReadModel = appointmentReadModel;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _changeDealStageOperationService = changeDealStageOperationService;
            _completeAppointmentAggregateService = completeAppointmentAggregateService;
        }

        public void Complete(long entityId)
        {            
            using (var scope = _operationScopeFactory.CreateSpecificFor<CompleteIdentity, Appointment>())
            {
                var appointment = _appointmentReadModel.GetAppointment(entityId);

                if (appointment.ScheduledStart.Date > DateTime.Now.Date)
                {
                    throw new BusinessLogicException(BLResources.ActivityClosingInFuturePeriodDenied);
                }
                       
                if (!_entityAccessService.HasActivityUpdateAccess<Appointment>(_userContext.Identity, entityId, appointment.OwnerCode))
                {
                    throw new SecurityException(string.Format("{0}: {1}", appointment.Header, BLResources.SecurityAccessDenied));
                }                

                _completeAppointmentAggregateService.Complete(appointment);

                var appointmentRegardingObjects = _appointmentReadModel.GetRegardingObjects(entityId);
                UpdateDealStage(appointmentRegardingObjects, appointment);

                scope.Updated<Appointment>(entityId);
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
        private void UpdateDealStage(IEnumerable<AppointmentRegardingObject> regardingObjects, Appointment phonecall)
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
