using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Read;
using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    public sealed class ActivityReadService : IActivityReadService
    {
        private readonly IAppointmentReadModel _appointmentReadModel;
        private readonly ILetterReadModel _letterReadModel;
        private readonly IPhonecallReadModel _phonecallReadModel;
        private readonly ITaskReadModel _taskReadModel;

        public ActivityReadService(
            IAppointmentReadModel appointmentReadModel, 
            ILetterReadModel letterReadModel, 
            IPhonecallReadModel phonecallReadModel, 
            ITaskReadModel taskReadModel)
        {
            _appointmentReadModel = appointmentReadModel;
            _letterReadModel = letterReadModel;
            _phonecallReadModel = phonecallReadModel;
            _taskReadModel = taskReadModel;
        }

        public bool CheckIfRelatedActivitiesExists(EntityName entityName, long entityId)
        {
            return
                _appointmentReadModel.CheckIfRelatedActivitiesExists(entityName, entityId)
                || _letterReadModel.CheckIfRelatedActivitiesExists(entityName, entityId)
                || _phonecallReadModel.CheckIfRelatedActivitiesExists(entityName, entityId)
                || _taskReadModel.CheckIfRelatedActivitiesExists(entityName, entityId);
        }

        public bool CheckIfRelatedActiveActivitiesExists(EntityName entityName, long entityId)
        {
            return
                _appointmentReadModel.CheckIfRelatedActiveActivitiesExists(entityName, entityId)
                || _letterReadModel.CheckIfRelatedActiveActivitiesExists(entityName, entityId)
                || _phonecallReadModel.CheckIfRelatedActiveActivitiesExists(entityName, entityId)
                || _taskReadModel.CheckIfRelatedActiveActivitiesExists(entityName, entityId);
        }
    }
}