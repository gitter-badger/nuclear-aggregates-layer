using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    public sealed class ModifyAppointmentService : IModifyBusinessModelEntityService<Appointment>
    {
        private readonly IAppointmentReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<Appointment> _activityObtainer;
        private readonly ICreateAppointmentAggregateService _createOperationService;
        private readonly IUpdateAppointmentAggregateService _updateOperationService;
        private readonly IChangeDealStageOperationService _changeDealStageOperationService;

        public ModifyAppointmentService(
            IAppointmentReadModel readModel,
            IBusinessModelEntityObtainer<Appointment> obtainer,
            ICreateAppointmentAggregateService createOperationService,
            IUpdateAppointmentAggregateService updateOperationService,
            IChangeDealStageOperationService changeDealStageOperationService)
        {
            _readModel = readModel;
            _activityObtainer = obtainer;
            _createOperationService = createOperationService;
            _updateOperationService = updateOperationService;
            _changeDealStageOperationService = changeDealStageOperationService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var appointmentDto = (AppointmentDomainEntityDto)domainEntityDto;
            var appointment = _activityObtainer.ObtainBusinessModelEntity(domainEntityDto);

            if (appointment.ScheduledStart > appointment.ScheduledEnd)
            {
                throw new NotificationException(BLResources.ModifyAppointmentService_ScheduleRangeIsIncorrect);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                IEnumerable<AppointmentRegardingObject> oldRegardingObjects;
                IEnumerable<AppointmentAttendee> oldAttendees;
                if (appointment.IsNew())
                {
                    _createOperationService.Create(appointment);
                    oldRegardingObjects = null;
                    oldAttendees = null;
                }
                else
                {
                    _updateOperationService.Update(appointment);
                    oldRegardingObjects = _readModel.GetRegardingObjects(appointment.Id);
                    oldAttendees = _readModel.GetAttendees(appointment.Id);
                }

                _updateOperationService.UpdateAttendees(appointment,
                                                        oldAttendees,
                                                        appointment.ReferencesIfAny<Appointment, AppointmentAttendee>(appointmentDto.Attendees));

                _updateOperationService.ChangeRegardingObjects(appointment,
                                                               oldRegardingObjects,
                                                               appointment.ReferencesIfAny<Appointment, AppointmentRegardingObject>(appointmentDto.RegardingObjects));

                if (appointment.Status == ActivityStatus.Completed)
                {
                    UpdateDealStage(appointmentDto);
                }

                transaction.Complete();

                return appointment.Id;
            }
        }

        /// <summary>
        /// Tries to update the related deal stage if any.
        /// </summary>
        /// <remarks>
        /// See the specs on https://confluence.2gis.ru/pages/viewpage.action?pageId=48464616.
        /// </remarks>
        private void UpdateDealStage(AppointmentDomainEntityDto appointmentDto)
        {
            var dealRef = appointmentDto.RegardingObjects.FirstOrDefault(x => x.EntityTypeId.Equals(EntityType.Instance.Deal().Id));
            if (dealRef == null || !dealRef.Id.HasValue)
            {
                return;
            }

            var dealId = dealRef.Id.Value;
            var purpose = appointmentDto.Purpose;

            var newDealStage = ConvertToStage(purpose);
            if (newDealStage == DealStage.None)
            {
                return;
            }

            _changeDealStageOperationService.Change(dealId, newDealStage);
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
    }
}