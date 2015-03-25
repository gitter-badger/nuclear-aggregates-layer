using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Reopen;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Reopen
{
    public class ReopenAppointmentAggregateService : IReopenAppointmentAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Appointment> _repository;

        public ReopenAppointmentAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Appointment> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void Reopen(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException("appointment");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ReopenIdentity, Appointment>())
            {
                appointment.Status = ActivityStatus.InProgress;

                _repository.Update(appointment);
                _repository.Save();

                operationScope.Updated<Appointment>(appointment.Id);
                operationScope.Complete();
            }
        }
    }
}
