using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Reopen;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Reopen
{
    public class ReopenAppointmentAggregateService : IReopenAppointmentAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;

        private readonly IActionLogger _actionLogger;

        private readonly IRepository<Appointment> _repository;

        public ReopenAppointmentAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IActionLogger actionLogger,
            IRepository<Appointment> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _actionLogger = actionLogger;
            _repository = repository;
        }

        public void Reopen(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException("appointment");
            }

            if (appointment.Status == ActivityStatus.InProgress)
            {
                return;
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ReopenIdentity, Appointment>())
            {
                var originalValue = appointment.Status;
                appointment.Status = ActivityStatus.InProgress;

                _repository.Update(appointment);
                _repository.Save();

                _actionLogger.LogChanges(appointment, x => x.Status, originalValue, appointment.Status);

                operationScope.Updated<Appointment>(appointment.Id);
                operationScope.Complete();
            }
        }
    }
}
