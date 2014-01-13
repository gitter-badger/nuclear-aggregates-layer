using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Aggregates.Activities;
using DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    public class ModifyPhonecallService : IModifyBusinessModelEntityService<Phonecall>
    {
        private readonly IActivityAggregateService _activityAggregateService;
        private readonly IBusinessModelEntityObtainer<Phonecall> _phonecallObtainer;
        private readonly IActivityReadModel _activityReadModel;

        public ModifyPhonecallService(IActivityAggregateService activityAggregateService, IBusinessModelEntityObtainer<Phonecall> phonecallObtainer, IActivityReadModel activityReadModel)
        {
            _activityAggregateService = activityAggregateService;
            _phonecallObtainer = phonecallObtainer;
            _activityReadModel = activityReadModel;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var phonecall = _phonecallObtainer.ObtainBusinessModelEntity(domainEntityDto);
            var activityInstanceDto = _activityReadModel.GetActivityInstanceDto(phonecall);
            if (phonecall.IsNew())
            {
                return _activityAggregateService.CreateActivity(activityInstanceDto.ActivityInstance, activityInstanceDto.ActivityPropretyInstances);
            }
            
            _activityAggregateService.UpdateActivity(activityInstanceDto.ActivityInstance, activityInstanceDto.ActivityPropretyInstances);
            return phonecall.Id;
        }
    }
}
