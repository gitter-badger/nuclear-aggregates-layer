using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public sealed class AssignAppointmentAggregateService : IAssignAppointmentAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Appointment> _repository;

        public AssignAppointmentAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Appointment> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
        }

        public void Assign(Appointment appointment, long ownerCode)
        {
            if (appointment == null)
            {
                throw new ArgumentNullException("appointment");
            }

            if (appointment.Status != ActivityStatus.InProgress)
            {
                throw new BusinessLogicException(BLResources.CannotAssignActivityNotInProgress);
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignIdentity, Appointment>())
            {
                appointment.OwnerCode = ownerCode;

                _repository.Update(appointment);
                _repository.Save();

                operationScope.Updated<Appointment>(appointment.Id);
                operationScope.Complete();
            }
        }
    }
}