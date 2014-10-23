using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Processing.Final.HotClient;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Strategies;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Processing.Final
{
    public sealed class ReplicateHotClientPerformedOperationsFinalProcessingStrategy :
        MessageProcessingStrategyBase
            <FinalReplicateHotClientPerformedOperationsFlow, PerformedOperationsFinalProcessingMessage, ReplicateHotClientFinalProcessingResultsMessage>
    {
        private readonly IFirmReadModel _firmReadModel;
        private readonly IGetHotClientTaskToReplicateOperationService _getHotClientTaskToReplicateOperationService;

        public ReplicateHotClientPerformedOperationsFinalProcessingStrategy(IFirmReadModel firmReadModel,
                                                                   IGetHotClientTaskToReplicateOperationService getHotClientTaskToReplicateOperationService)
        {
            _firmReadModel = firmReadModel;
            _getHotClientTaskToReplicateOperationService = getHotClientTaskToReplicateOperationService;
        }

        protected override ReplicateHotClientFinalProcessingResultsMessage Process(PerformedOperationsFinalProcessingMessage message)
        {
            // TODO {s.pomadin, 24.09.2014}: тут подозрительно то что фактически делается одно и то же дважды в gethotclienttask + здесь используется _firmReadModel.GetHotClientRequest(message.EntityId); - возможно нужен сервис/метод вида ПодготовьШаблонTaskДляЗавершенияОбработкиЗаявки(экземпляр заявки) + само присутсвие слова Replicate в названии сервиса уже не отвечает реалиям этой ветки - теперь мастер система по дествиям ERM, а не MsCRM
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