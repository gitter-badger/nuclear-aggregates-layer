using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public sealed class CreateTaskAggregateService : ICreateTaskAggregateService
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

        public void Create(Task task)
        {
            if (!task.IsNew())
            {
                throw new ArgumentException(ActivityHasAlreadyTheIdentityMessage, "task");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Task>())
            {
                _identityProvider.SetFor(task);

                _repository.Add(task);
                _repository.Save();
                
                operationScope.Added<Task>(task.Id);
                operationScope.Complete();
            }
        }
    }
}