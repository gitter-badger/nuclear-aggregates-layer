using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Processing.Final.HotClient;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Processing.Final
{
    public class ReplicateHotClientPerformedOperationsFinalProcessor :
        MessageProcessingStrategyBase
            <FinalReplicateHotClientPerformedOperationsFlow, PerformedOperationsFinalProcessingMessage, ReplicateHotClientFinalProcessingResultsMessage>
    {
        private readonly IFirmReadModel _firmReadModel;
        private readonly IGetHotClientTaskToReplicateOperationService _getHotClientTaskToReplicateOperationService;

        public ReplicateHotClientPerformedOperationsFinalProcessor(IFirmReadModel firmReadModel,
                                                                   IGetHotClientTaskToReplicateOperationService getHotClientTaskToReplicateOperationService)
        {
            _firmReadModel = firmReadModel;
            _getHotClientTaskToReplicateOperationService = getHotClientTaskToReplicateOperationService;
        }

        protected override ReplicateHotClientFinalProcessingResultsMessage Process(PerformedOperationsFinalProcessingMessage message)
        {
            var task = _getHotClientTaskToReplicateOperationService.GetHotClientTask(message.EntityId);
            var entity = _firmReadModel.GetHotClientRequest(message.EntityId);

            return new ReplicateHotClientFinalProcessingResultsMessage
                {
                    Owner = task.TaskOwner,
                    HotClient = task.HotClientDto,
                    RegardingObject = task.Regarding,
                    RequestEntity = entity
                };
        }
    }
}