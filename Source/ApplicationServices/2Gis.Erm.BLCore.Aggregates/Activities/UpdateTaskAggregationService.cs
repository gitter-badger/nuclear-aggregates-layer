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
    public sealed class UpdateTaskAggregationService : IUpdateTaskAggregateService, IUpdateRegardingObjectAggregateService<Task>
    {
        private const string ActivityHasNoTheIdentityMessage = "The task has no the identity.";

        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Task> _repository;
        private readonly IRepository<RegardingObject<Task>> _referenceRepository;

        public UpdateTaskAggregationService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Task> repository,
            IRepository<RegardingObject<Task>> referenceRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
            _referenceRepository = referenceRepository;
        }

        public void Update(Task task)
        {
            if (task.Id == 0)
            {
                throw new ArgumentException(ActivityHasNoTheIdentityMessage, "task");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Task>())
            {
                _repository.Update(task);
                operationScope.Updated<Task>(task.Id);

                _repository.Save();
                operationScope.Complete();
            }
        }

        public void ChangeRegardingObjects(IEnumerable<RegardingObject<Task>> oldReferences, IEnumerable<RegardingObject<Task>> newReferences)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<AssignRegardingObjectIdentity, RegardingObject<Task>>())
            {
                _referenceRepository.Update(oldReferences, newReferences);

                //operationScope.Updated<RegardingObject<Appointment>>(newReferences);

                operationScope.Complete();
            }
        }
    }
}