using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.Operations.Cancel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities.Operations.Cancel
{
    public class CancelAppointmentAggregateService : ICancelAppointmentAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<Appointment> _repository;

        public CancelAppointmentAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<Appointment> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void Cancel(Appointment appointment)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException("appointment");
            }

            if (appointment.Status != ActivityStatus.InProgress)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCancelFinishedOrClosedActivity, appointment.Header));
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CancelIdentity, Appointment>())
            {                
                appointment.Status = ActivityStatus.Canceled;

                _repository.Update(appointment);
                _repository.Save();

                operationScope.Updated<Appointment>(appointment.Id);
                operationScope.Complete();
            }
        }
    }
}
