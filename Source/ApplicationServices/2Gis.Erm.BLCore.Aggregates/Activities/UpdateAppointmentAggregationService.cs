using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Activity;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public sealed class UpdateAppointmentAggregationService : IUpdateAppointmentAggregateService, IUpdateRegardingObjectAggregateService<Appointment>
    {
        private const string ActivityHasNoTheIdentityMessage = "The appointment has no the identity.";

        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Appointment> _repository;
        private readonly IRepository<RegardingObject<Appointment>> _referenceRepository;

        public UpdateAppointmentAggregationService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Appointment> repository,
            IRepository<RegardingObject<Appointment>> referenceRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
            _referenceRepository = referenceRepository;
        }

        public void Update(Appointment appointment)
        {
            if (appointment.Id == 0)
            {
                throw new ArgumentException(ActivityHasNoTheIdentityMessage, "appointment");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Appointment>())
            {
                _repository.Update(appointment);
                operationScope.Updated<Appointment>(appointment.Id);

                _repository.Save();
                operationScope.Complete();
            }
        }

        public void ChangeRegardingObjects(IEnumerable<RegardingObject<Appointment>> oldReferences, IEnumerable<RegardingObject<Appointment>> newReferences)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignRegardingObjectIdentity, RegardingObject<Appointment>>())
            {
                _referenceRepository.Update(oldReferences, newReferences);

                //operationScope.Updated<RegardingObject<Appointment>>(newReferences);

                operationScope.Complete();
            }
        }
    }
}