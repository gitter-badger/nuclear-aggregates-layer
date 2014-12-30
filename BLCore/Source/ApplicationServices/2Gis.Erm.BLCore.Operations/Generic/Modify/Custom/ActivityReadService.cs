using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Read;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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

        public bool CheckIfActivityExistsRegarding(EntityName entityName, long entityId)
        {
            return
                _appointmentReadModel.CheckIfAppointmentExistsRegarding(entityName, entityId)
                || _letterReadModel.CheckIfLetterExistsRegarding(entityName, entityId)
                || _phonecallReadModel.CheckIfPhonecallExistsRegarding(entityName, entityId)
                || _taskReadModel.CheckIfTaskExistsRegarding(entityName, entityId);
        }

        public bool CheckIfOpenActivityExistsRegarding(EntityName entityName, long entityId)
        {
            return
                _appointmentReadModel.CheckIfOpenAppointmentExistsRegarding(entityName, entityId)
                || _letterReadModel.CheckIfOpenLetterExistsRegarding(entityName, entityId)
                || _phonecallReadModel.CheckIfOpenPhonecallExistsRegarding(entityName, entityId)
                || _taskReadModel.CheckIfOpenTaskExistsRegarding(entityName, entityId);
        }

        public IEnumerable<IEntity> LookupActivitiesRegarding(EntityName entityName, long entityId)
        {
            return
                _appointmentReadModel.LookupAppointmentsRegarding(entityName, entityId).Cast<IEntity>()
                                     .Concat(_letterReadModel.LookupLettersRegarding(entityName, entityId))
                                     .Concat(_phonecallReadModel.LookupPhonecallsRegarding(entityName, entityId))
                                     .Concat(_taskReadModel.LookupTasksRegarding(entityName, entityId));
        }
    }
}