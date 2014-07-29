using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    public class ModifyAppointmentService : IModifyBusinessModelEntityService<Appointment>
    {
        private readonly IActivityAggregateService _activityAggregateService;
        private readonly IBusinessModelEntityObtainer<Appointment> _appoinmentObtainer;
        private readonly IActivityReadModel _activityReadModel;

        public ModifyAppointmentService(IActivityAggregateService activityAggregateService, IBusinessModelEntityObtainer<Appointment> appoinmentObtainer, IActivityReadModel activityReadModel)
        {
            _activityAggregateService = activityAggregateService;
            _appoinmentObtainer = appoinmentObtainer;
            _activityReadModel = activityReadModel;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var appointment = _appoinmentObtainer.ObtainBusinessModelEntity(domainEntityDto);
            var activityInstanceDto = _activityReadModel.GetActivityInstanceDto(appointment);
            if (appointment.IsNew())
            {
                return _activityAggregateService.CreateActivity(activityInstanceDto.ActivityInstance, activityInstanceDto.ActivityPropretyInstances);
            }
            
            _activityAggregateService.UpdateActivity(activityInstanceDto.ActivityInstance, activityInstanceDto.ActivityPropretyInstances);
            return appointment.Id;
        }
    }
}
