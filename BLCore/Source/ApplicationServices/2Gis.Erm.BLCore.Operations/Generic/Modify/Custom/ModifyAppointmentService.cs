using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

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

        public ModifyAppointmentService(
            IAppointmentReadModel readModel,
            IBusinessModelEntityObtainer<Appointment> obtainer,
            IClientReadModel clientReadModel,
            IFirmReadModel firmReadModel,
            ICreateAppointmentAggregateService createOperationService,
            IUpdateAppointmentAggregateService updateOperationService)
        {
            _readModel = readModel;
            _activityObtainer = obtainer;
            _clientReadModel = clientReadModel;
            _firmReadModel = firmReadModel;
            _createOperationService = createOperationService;
            _updateOperationService = updateOperationService;
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

            if (appointmentDto.RegardingObjects.HasReferenceInReserve(EntityType.Instance.Client(), _clientReadModel.IsClientInReserve))
            {
                throw new BusinessLogicException(BLResources.CannotSaveActivityForClientInReserve);
            }

            if (appointmentDto.RegardingObjects.HasReferenceInReserve(EntityType.Instance.Firm(), _firmReadModel.IsFirmInReserve))
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

                _updateOperationService.UpdateAttendees(appointment, oldAttendees, appointment.ReferencesIfAny<Appointment, AppointmentAttendee>(appointmentDto.Attendees));

                _updateOperationService.ChangeRegardingObjects(
                    appointment,
                                                               oldRegardingObjects,
                                                               appointment.ReferencesIfAny<Appointment, AppointmentRegardingObject>(appointmentDto.RegardingObjects));
                _updateOperationService.ChangeOrganizer(appointment, oldOrganizer, appointment.ReferencesIfAny<Appointment, AppointmentOrganizer>(appointmentDto.Organizer));

                transaction.Complete();

                return appointment.Id;
            }
        }
    }
}