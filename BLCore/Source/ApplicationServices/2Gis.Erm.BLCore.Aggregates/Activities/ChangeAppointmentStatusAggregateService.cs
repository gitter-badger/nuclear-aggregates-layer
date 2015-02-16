using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public class ChangeAppointmentStatusAggregateService : IChangeAppointmentStatusAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Appointment> _repository;

        public ChangeAppointmentStatusAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Appointment> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }
        
        public void Change(Appointment appointment, ActivityStatus status)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException("appointment");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ChangeActivityStatusIdentity, Appointment>())
            {
                appointment.Status = status;

                _repository.Update(appointment);
                _repository.Save();

                operationScope.Updated<Appointment>(appointment.Id);
                operationScope.Complete();
            }
        }
    }
}
