using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Aggregates.Activities;
using DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    public class ModifyTaskService : IModifyBusinessModelEntityService<Task>
    {
        private readonly IActivityAggregateService _activityAggregateService;
        private readonly IActivityReadModel _activityReadModel;
        private readonly IBusinessModelEntityObtainer<Task> _taskObtainer;

        public ModifyTaskService(IBusinessModelEntityObtainer<Task> taskObtainer,
                                 IActivityReadModel activityReadModel,
                                 IActivityAggregateService activityAggregateService)
        {
            _activityReadModel = activityReadModel;
            _taskObtainer = taskObtainer;
            _activityAggregateService = activityAggregateService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var task = _taskObtainer.ObtainBusinessModelEntity(domainEntityDto);
            var activityInstanceDto = _activityReadModel.GetActivityInstanceDto(task);
            
            if (task.IsNew())
            {
                return _activityAggregateService.CreateActivity(activityInstanceDto.ActivityInstance, activityInstanceDto.ActivityPropretyInstances);
            }
            
            _activityAggregateService.UpdateActivity(activityInstanceDto.ActivityInstance, activityInstanceDto.ActivityPropretyInstances);
            return task.Id;
        }
    }
}
