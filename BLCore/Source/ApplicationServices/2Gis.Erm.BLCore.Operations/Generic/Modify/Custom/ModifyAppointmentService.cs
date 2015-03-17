using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
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
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    public sealed class ModifyAppointmentService : IModifyBusinessModelEntityService<Appointment>
    {
        private readonly IAppointmentReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<Appointment> _activityObtainer;

        private readonly IClientReadModel _clientReadModel;

        private readonly IFirmReadModel _firmReadModel;

        private readonly ICreateAppointmentAggregateService _createOperationService;
        private readonly IUpdateAppointmentAggregateService _updateOperationService;
        private readonly IChangeDealStageOperationService _changeDealStageOperationService;

        public ModifyAppointmentService(
            IAppointmentReadModel readModel,
            IBusinessModelEntityObtainer<Appointment> obtainer,
            IClientReadModel clientReadModel,
            IFirmReadModel firmReadModel,
            ICreateAppointmentAggregateService createOperationService,
            IUpdateAppointmentAggregateService updateOperationService,
            IChangeDealStageOperationService changeDealStageOperationService)
        {
            _readModel = readModel;
            _activityObtainer = obtainer;
            _clientReadModel = clientReadModel;
            _firmReadModel = firmReadModel;
            _createOperationService = createOperationService;
            _updateOperationService = updateOperationService;
            _changeDealStageOperationService = changeDealStageOperationService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var appointmentDto = (AppointmentDomainEntityDto)domainEntityDto;
            if (appointmentDto.RegardingObjects == null || !appointmentDto.RegardingObjects.Any())
            {
                throw new BusinessLogicException(BLResources.NoRegardingObjectValidationError);
            }

            var appointment = _activityObtainer.ObtainBusinessModelEntity(domainEntityDto);            
            if (appointment.ScheduledStart > appointment.ScheduledEnd)
            {
                throw new NotificationException(BLResources.ModifyAppointmentService_ScheduleRangeIsIncorrect);
            }

            if (appointmentDto.RegardingObjects.HasReferenceInReserve(EntityName.Client, _clientReadModel.IsClientInReserve))
            {
                throw new BusinessLogicException(BLResources.CannotSaveActivityForClientInReserve);
            }

            if (appointmentDto.RegardingObjects.HasReferenceInReserve(EntityName.Firm, _firmReadModel.IsFirmInReserve))
            {
                throw new BusinessLogicException(BLResources.CannotSaveActivityForFirmInReserve);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                IEnumerable<AppointmentRegardingObject> oldRegardingObjects;
                IEnumerable<AppointmentAttendee> oldAttendees;
                AppointmentOrganizer oldOrganizer;
                if (appointment.IsNew())
                {
                    _createOperationService.Create(appointment);
                    oldRegardingObjects = null;
                    oldAttendees = null;
                    oldOrganizer = null;
                }
                else
                {
                    _updateOperationService.Update(appointment);
                    oldRegardingObjects = _readModel.GetRegardingObjects(appointment.Id);
                    oldAttendees = _readModel.GetAttendees(appointment.Id);
                    oldOrganizer = _readModel.GetOrganizer(appointment.Id);
                }

                _updateOperationService.UpdateAttendees(appointment,
                                                        oldAttendees,
                                                        appointment.ReferencesIfAny<Appointment, AppointmentAttendee>(appointmentDto.Attendees));

                _updateOperationService.ChangeRegardingObjects(appointment,
                                                               oldRegardingObjects,
                                                               appointment.ReferencesIfAny<Appointment, AppointmentRegardingObject>(appointmentDto.RegardingObjects));

                _updateOperationService.ChangeOrganizer(appointment,
                                                        oldOrganizer,
                                                        appointment.ReferencesIfAny<Appointment, AppointmentOrganizer>(appointmentDto.Organizer));

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
            var dealRef = appointmentDto.RegardingObjects.FirstOrDefault(x => x.EntityName == EntityName.Deal);
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

        private static DealStage ConvertToStage(AppointmentPurpose purpose)
        {
            switch (purpose)
            {
                case AppointmentPurpose.FirstCall:
                    return DealStage.CollectInformation;

                case AppointmentPurpose.ProductPresentation:
                case AppointmentPurpose.OpportunitiesPresentation:
                    return DealStage.HoldingProductPresentation;

                case AppointmentPurpose.OfferApproval:
                case AppointmentPurpose.DecisionApproval:
                    return DealStage.MatchAndSendProposition;

                default:
                    return DealStage.None;
            }
        }
    }
}