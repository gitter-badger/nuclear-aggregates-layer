using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Complete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Complete;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Complete
{
    public class CompleteAppointmentAggregateService : ICompleteAppointmentAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;

        private readonly IActionLogger _actionLogger;

        private readonly IRepository<Appointment> _repository;

        public CompleteAppointmentAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IActionLogger actionLogger,
            IRepository<Appointment> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _actionLogger = actionLogger;
            _repository = repository;
        }

        public void Complete(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException("appointment");
            }

            if (appointment.Status == ActivityStatus.Completed)
            {
                return;
            }

            if (appointment.Status != ActivityStatus.InProgress)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCompleteFinishedOrClosedActivity, appointment.Header));
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CompleteIdentity, Appointment>())
            {
                var originalValue = appointment.Status;
                appointment.Status = ActivityStatus.Completed;

                _repository.Update(appointment);
                _repository.Save();

                _actionLogger.LogChanges(appointment, x => x.Status, originalValue, appointment.Status);

                operationScope.Updated<Appointment>(appointment.Id);
                operationScope.Complete();
            }
        }
    }
}
