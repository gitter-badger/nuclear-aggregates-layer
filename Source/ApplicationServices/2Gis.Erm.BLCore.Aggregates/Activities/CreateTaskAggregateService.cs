using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public sealed class CreateTaskAggregateService : IAggregateRootRepository<Task>, ICreateAggregateRepository<Task>
    {
        private const string ActivityHasAlreadyTheIdentityMessage = "The task has already the identity.";

        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<Task> _repository;

        public CreateTaskAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IIdentityProvider identityProvider,
            IRepository<Task> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _identityProvider = identityProvider;
            _repository = repository;
        }

        public int Create(Task task)
        {
            if (!task.IsNew())
            {
                throw new ArgumentException(ActivityHasAlreadyTheIdentityMessage, "task");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Task>())
            {
                _identityProvider.SetFor(task);

                _repository.Add(task);
                operationScope.Added<Task>(task.Id);

                var count = _repository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}