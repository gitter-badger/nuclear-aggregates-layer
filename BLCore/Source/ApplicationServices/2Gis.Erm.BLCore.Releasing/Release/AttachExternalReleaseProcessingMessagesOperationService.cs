using DoubleGis.Erm.BLCore.API.Aggregates.Releases.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Releases.ReadModel;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public sealed class AttachExternalReleaseProcessingMessagesOperationService : IAttachExternalReleaseProcessingMessagesOperationService
    {
        private readonly IReleaseReadModel _releaseReadModel;
        private readonly IReleaseAttachProcessingMessagesAggregateService _attachProcessingMessagesAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _logger;

        public AttachExternalReleaseProcessingMessagesOperationService(
            IReleaseReadModel releaseReadModel,
            IReleaseAttachProcessingMessagesAggregateService attachProcessingMessagesAggregateService,
            IOperationScopeFactory scopeFactory,
            ITracer logger)
        {
            _releaseReadModel = releaseReadModel;
            _attachProcessingMessagesAggregateService = attachProcessingMessagesAggregateService;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void Attach(long releaseId, ExternalReleaseProcessingMessage[] messages)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<AttachExternalReleaseProcessingMessagesIdentity>())
            {
                var release = _releaseReadModel.GetReleaseInfo(releaseId);
                if (release == null)
                {
                    throw new NotificationException(string.Format("Can't save release external processing results. Release entry for specifed release id " + releaseId + " not found"));
                }

                _logger.InfoFormat("Attaching external release processing results. Release id: {0}. Messages count:{1}.", release.Id, messages.Length);
                _attachProcessingMessagesAggregateService.SaveExternalMessages(release, messages);
                _logger.InfoFormat("External release processing results attached successfully.", release.Id, messages.Length);

                scope.Complete();
            }
        }
    }
}