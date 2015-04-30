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
    public sealed class CreateLetterAggregateService : ICreateLetterAggregateService
    {
        private const string ActivityHasAlreadyTheIdentityMessage = "The letter has already the identity.";

        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<Letter> _repository;

        public CreateLetterAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IIdentityProvider identityProvider,
            IRepository<Letter> repository)
        {
            _operationScopeFactory = operationScopeFactory;
            _identityProvider = identityProvider;
            _repository = repository;
        }

        public void Create(Letter letter)
        {
            if (!letter.IsNew())
            {
                throw new ArgumentException(ActivityHasAlreadyTheIdentityMessage, "letter");
            }

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Letter>())
            {
                _identityProvider.SetFor(letter);

                _repository.Add(letter);
                _repository.Save();

                operationScope.Added<Letter>(letter.Id);
                operationScope.Complete();
            }
        }
    }
}