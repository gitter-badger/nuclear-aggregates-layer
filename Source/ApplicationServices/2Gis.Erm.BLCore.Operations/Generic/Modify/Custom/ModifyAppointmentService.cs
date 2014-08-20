using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
	public sealed class ModifyAppointmentService : IModifyBusinessModelEntityService<Appointment>
	{
		private readonly IActivityReadModel _readModel;
		private readonly IBusinessModelEntityObtainer<Appointment> _activityObtainer;
		private readonly ICreateAggregateRepository<Appointment> _createOperationService;
		private readonly IUpdateAppointmentAggregateService _updateOperationService;
		private readonly IUpdateRegardingObjectAggregateService<Appointment> _updateRegardingObjectService;

		public ModifyAppointmentService(
			IActivityReadModel readModel,
			IBusinessModelEntityObtainer<Appointment> obtainer,
			ICreateAggregateRepository<Appointment> createOperationService,
			IUpdateAppointmentAggregateService updateOperationService,
			IUpdateRegardingObjectAggregateService<Appointment> updateRegardingObjectService)
		{
			_readModel = readModel;
			_activityObtainer = obtainer;
			_createOperationService = createOperationService;
			_updateOperationService = updateOperationService;
			_updateRegardingObjectService = updateRegardingObjectService;
		}

		public long Modify(IDomainEntityDto domainEntityDto)
		{
			var appointmentDto = (AppointmentDomainEntityDto)domainEntityDto;
			var appointment = _activityObtainer.ObtainBusinessModelEntity(domainEntityDto);

			IEnumerable<RegardingObject<Appointment>> oldRegardingObjects;
			if (appointment.IsNew())
			{
				_createOperationService.Create(appointment);
				oldRegardingObjects = null;
			}
			else
			{
				_updateOperationService.Update(appointment);
				oldRegardingObjects = _readModel.GetRegardingObjects<Appointment>(appointment.Id);
			}

			_updateRegardingObjectService.ChangeRegardingObjects(oldRegardingObjects, new[]
				{
					appointment.ReferenceIfAny(EntityName.Client, appointmentDto.ClientRef.Id),
					appointment.ReferenceIfAny(EntityName.Contact, appointmentDto.ContactRef.Id),
					appointment.ReferenceIfAny(EntityName.Deal, appointmentDto.DealRef.Id),
					appointment.ReferenceIfAny(EntityName.Firm, appointmentDto.FirmRef.Id)
				}.Where(x => x != null));

			return appointment.Id;
		}
	}
}