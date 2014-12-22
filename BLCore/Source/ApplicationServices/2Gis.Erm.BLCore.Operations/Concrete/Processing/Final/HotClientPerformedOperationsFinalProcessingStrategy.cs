using DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Processing.Final.HotClient;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Processing.Final
{
    public sealed class HotClientPerformedOperationsFinalProcessingStrategy :
        MessageProcessingStrategyBase<FinalProcessingOfHotClientPerformedOperationsFlow, PerformedOperationsFinalProcessingMessage, HotClientFinalProcessingResultsMessage>
    {
        private readonly IGetHotClientRequestOperationService _getHotClientRequestOperationService;

        public HotClientPerformedOperationsFinalProcessingStrategy(IGetHotClientRequestOperationService getHotClientRequestOperationService)
        {
            _getHotClientRequestOperationService = getHotClientRequestOperationService;
        }

        protected override HotClientFinalProcessingResultsMessage Process(PerformedOperationsFinalProcessingMessage message)
        {
            var hotClientTaskDto = _getHotClientRequestOperationService.GetHotClientTask(message.EntityId);

            return new HotClientFinalProcessingResultsMessage
                {
                    HotClientRequest = hotClientTaskDto.HotClientDto,
                    OwnerId = hotClientTaskDto.TaskOwner,
                    RegardingObject = hotClientTaskDto.Regarding,
                };
        }
    }
}