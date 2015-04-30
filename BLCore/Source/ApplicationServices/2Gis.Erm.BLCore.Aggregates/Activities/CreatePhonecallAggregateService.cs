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
    public sealed class CreatePhonecallAggregateService : ICreatePhonecallAggregateService
    {
        private const string ActivityHasAlreadyTheIdentityMessage = "The phonecall has already the identity.";

        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<Phonecall> _repository;

        public CreatePhonecallAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IIdentityProvider identityProvider,
            IRepository<Phonecall> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _identityProvider = identityProvider;
            _repository = repository;
        }

        public void Create(Phonecall phonecall)
        {
            if (!phonecall.IsNew())
            {
                throw new ArgumentException(ActivityHasAlreadyTheIdentityMessage, "phonecall");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Phonecall>())
            {
                _identityProvider.SetFor(phonecall);

                _repository.Add(phonecall);
                _repository.Save();
                
                operationScope.Added<Phonecall>(phonecall.Id);
                operationScope.Complete();
            }
        }
    }
}