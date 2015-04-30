using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public sealed class CreateAppointmentAggregateService : ICreateAppointmentAggregateService
    {
        private const string ActivityHasAlreadyTheIdentityMessage = "The appointment has already the identity.";

        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<Appointment> _repository;

        public CreateAppointmentAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IIdentityProvider identityProvider,
            IRepository<Appointment> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _identityProvider = identityProvider;
            _repository = repository;
        }

        public void Create(Appointment appointment)
        {
            if (!appointment.IsNew())
            {
                throw new ArgumentException(ActivityHasAlreadyTheIdentityMessage, "appointment");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Appointment>())
            {
                _identityProvider.SetFor(appointment);

                _repository.Add(appointment);
                _repository.Save();
                
                operationScope.Added<Appointment>(appointment.Id);
                operationScope.Complete();
            }
        }
    }
}