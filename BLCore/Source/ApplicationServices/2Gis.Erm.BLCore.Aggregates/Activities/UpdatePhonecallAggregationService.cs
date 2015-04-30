using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Activities
{
    public sealed class UpdatePhonecallAggregationService : IUpdatePhonecallAggregateService
    {
        private const string ActivityHasNoTheIdentityMessage = "The phonecall has no the identity.";

        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Phonecall> _repository;
        private readonly IRepository<PhonecallRegardingObject> _referenceRepository;
        private readonly IRepository<PhonecallRecipient> _recipientRepository;

        public UpdatePhonecallAggregationService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<Phonecall> repository,
            IRepository<PhonecallRegardingObject> referenceRepository,
            IRepository<PhonecallRecipient> recipientRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _repository = repository;
            _referenceRepository = referenceRepository;
            _recipientRepository = recipientRepository;
        }

        public void Update(Phonecall phonecall)
        {
            CheckPhonecall(phonecall);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Phonecall>())
            {
                _repository.Update(phonecall);
                operationScope.Updated<Phonecall>(phonecall.Id);

                _repository.Save();
                operationScope.Complete();
            }
        }

        public void ChangeRegardingObjects(Phonecall phonecall, IEnumerable<PhonecallRegardingObject> oldReferences, IEnumerable<PhonecallRegardingObject> newReferences)
        {
            CheckPhonecall(phonecall);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Phonecall>())
            {
                _referenceRepository.Update<Phonecall, PhonecallRegardingObject>(oldReferences, newReferences);

                operationScope.Updated<Phonecall>(phonecall.Id);
                operationScope.Complete();
            }
        }

        public void ChangeRecipient(Phonecall phonecall, PhonecallRecipient oldRecipient, PhonecallRecipient newRecipient)
        {
            CheckPhonecall(phonecall);

            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Phonecall>())
            {
                _recipientRepository.Update<Phonecall, PhonecallRecipient>(new[] { oldRecipient }, new[] { newRecipient });

                operationScope.Updated<Phonecall>(phonecall.Id);
                operationScope.Complete();
            }
        }

        private static void CheckPhonecall(Phonecall phonecall)
        {
            if (phonecall == null)
            {
                throw new ArgumentNullException("phonecall");
            }

            if (phonecall.Id == 0)
            {
                throw new ArgumentException(ActivityHasNoTheIdentityMessage, "phonecall");
            }
        }
    }
}