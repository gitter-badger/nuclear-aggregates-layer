﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public sealed class UpdateLetterAggregationService : IUpdateLetterAggregateService
    {
        private const string ActivityHasNoTheIdentityMessage = "The letter has no the identity.";

        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Letter> _repository;
        private readonly IRepository<LetterRegardingObject> _referenceRepository;
        private readonly IRepository<LetterSender> _senderRepository;
        private readonly IRepository<LetterRecipient> _recipientRepository;

        public UpdateLetterAggregationService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Letter> repository,
            IRepository<LetterRegardingObject> referenceRepository,
            IRepository<LetterSender> senderRepository,
            IRepository<LetterRecipient> recipientRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
            _referenceRepository = referenceRepository;
            _senderRepository = senderRepository;
            _recipientRepository = recipientRepository;
        }

        private static void CheckLetter(Letter letter)
        {
            if (letter == null)
            {
                throw new ArgumentNullException("letter");
            }
            if (letter.Id == 0)
            {
                throw new ArgumentException(ActivityHasNoTheIdentityMessage, "letter");
            }
        }

        public void Update(Letter letter)
        {
            CheckLetter(letter);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Letter>())
            {
                _repository.Update(letter);
                _repository.Save();
                
                operationScope.Updated<Letter>(letter.Id);
                operationScope.Complete();
            }
        }

        public void ChangeRegardingObjects(Letter letter, IEnumerable<LetterRegardingObject> oldReferences, IEnumerable<LetterRegardingObject> newReferences)
        {
            CheckLetter(letter);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Letter>())
            {
                _referenceRepository.Update<Letter, LetterRegardingObject>(oldReferences, newReferences);

                operationScope.Updated<Letter>(letter.Id);
                operationScope.Complete();
            }
        }

        public void ChangeSender(Letter letter, LetterSender oldSender, LetterSender newSender)
        {
            CheckLetter(letter);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Letter>())
            {
                _senderRepository.Update<Letter, LetterSender>(Enumerate(oldSender), Enumerate(newSender));

                operationScope.Updated<Letter>(letter.Id);
                operationScope.Complete();
            }
        }

        public void ChangeRecipient(Letter letter, LetterRecipient oldRecipient, LetterRecipient newRecipient)
        {
            CheckLetter(letter);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Letter>())
            {
                _recipientRepository.Update<Letter, LetterRecipient>(Enumerate(oldRecipient), Enumerate(newRecipient));

                operationScope.Updated<Letter>(letter.Id);
                operationScope.Complete();
            }
        }

        private static IEnumerable<TEntityReference> Enumerate<TEntityReference>(TEntityReference reference)
            where TEntityReference : EntityReference<Letter>
        {
            if (reference == null)
            {
                yield break;
            }
            yield return reference;
        }
    }
}