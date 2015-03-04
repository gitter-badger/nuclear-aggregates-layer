using DoubleGis.Erm.BLCore.API.Aggregates.LocalMessages;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.LocalMessages
{
    public sealed class ReprocessLocalMessagesHandler : RequestHandler<ReprocessLocalMessagesRequest, EmptyResponse>
    {
        private readonly ICommonLog _logger;
        private readonly ILocalMessageRepository _localMessageRepository;

        public ReprocessLocalMessagesHandler(ICommonLog logger, ILocalMessageRepository localMessageRepository)
        {
            _logger = logger;
            _localMessageRepository = localMessageRepository;
        }

        protected override EmptyResponse Handle(ReprocessLocalMessagesRequest request)
        {
            var localMessages = _localMessageRepository.GetLongProcessingMessages(request.PeriodInMinutes);

            foreach (var localMessage in localMessages)
            {
                var reprocessResult = string.Format("Сообщение [{0}] было отправлено на повторную обработку", localMessage.Id);
                _localMessageRepository.SetResult(localMessage, LocalMessageStatus.WaitForProcess, new[] { reprocessResult }, 0);

                _logger.Info(reprocessResult);
            }

            return Response.Empty;
        }
    }
}