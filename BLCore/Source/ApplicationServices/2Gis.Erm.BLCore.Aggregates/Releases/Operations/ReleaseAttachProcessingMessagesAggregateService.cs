using System;
using System.Collections;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Releases.Operations;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Releases.Operations
{
    public sealed class ReleaseAttachProcessingMessagesAggregateService : IReleaseAttachProcessingMessagesAggregateService
    {
        private readonly IRepository<ReleaseValidationResult> _resultsRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public ReleaseAttachProcessingMessagesAggregateService(
            IRepository<ReleaseValidationResult> resultsRepository,
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _resultsRepository = resultsRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void SaveInternalMessages(ReleaseInfo release, IEnumerable<ReleaseProcessingMessage> messages)
        {
            Save(release, messages, FromReleaseProcessingMessage);
        }

        public void SaveExternalMessages(ReleaseInfo release, IEnumerable<ExternalReleaseProcessingMessage> messages)
        {
            Save(release, messages, FromExternalReleaseProcessingMessage);
        }

        private void Save(ReleaseInfo release, IEnumerable messages, Func<ReleaseInfo, object, ReleaseValidationResult> messageProjector)
        {
            if (messages == null)
            {
                return;
            }

            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, ReleaseValidationResult>())
            {
                foreach (var message in messages)
                {
                    var resultMessage = messageProjector(release, message);
                    _identityProvider.SetFor(resultMessage);
                    _resultsRepository.Add(resultMessage);
                    scope.Added<ReleaseValidationResult>(resultMessage.Id);
                }

                _resultsRepository.Save();
                scope.Complete();
            }
        }

        private ReleaseValidationResult FromReleaseProcessingMessage(ReleaseInfo release, object source)
        {
            var message = (ReleaseProcessingMessage)source;
            return new ReleaseValidationResult
            {
                IsBlocking = message.IsBlocking,
                Message = message.Message,
                OrderId = message.OrderId,
                ReleaseInfoId = release.Id,
                RuleCode = message.RuleCode
            };
        }

        private ReleaseValidationResult FromExternalReleaseProcessingMessage(ReleaseInfo release, object source)
        {
            var message = (ExternalReleaseProcessingMessage)source;
            return new ReleaseValidationResult
            {
                IsBlocking = message.IsBlocking,
                Message = message.Description,
                OrderId = null,
                ReleaseInfoId = release.Id,
                RuleCode = message.MessageType
            };
        }
    }
}