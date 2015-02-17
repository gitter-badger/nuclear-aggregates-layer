using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    public sealed class ModifyAppointmentService : IModifyBusinessModelEntityService<Appointment>
    {
        private readonly IAppointmentReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<Appointment> _activityObtainer;
        private readonly ICreateAppointmentAggregateService _createOperationService;
        private readonly IUpdateAppointmentAggregateService _updateOperationService;

        public ModifyAppointmentService(
            IAppointmentReadModel readModel,
            IBusinessModelEntityObtainer<Appointment> obtainer,
            ICreateAppointmentAggregateService createOperationService,
            IUpdateAppointmentAggregateService updateOperationService)
        {
            _readModel = readModel;
            _activityObtainer = obtainer;
            _createOperationService = createOperationService;
            _updateOperationService = updateOperationService;
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
                transaction.Complete();

                return appointment.Id;
            }
        }
    }
}