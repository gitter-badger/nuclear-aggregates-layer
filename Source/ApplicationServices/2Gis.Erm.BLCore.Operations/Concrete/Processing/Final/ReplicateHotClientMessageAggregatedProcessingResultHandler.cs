using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Processing.Final.HotClient;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Handlers;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing.Stages;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Processing.Final
{
    public sealed class ReplicateHotClientMessageAggregatedProcessingResultHandler : IMessageAggregatedProcessingResultsHandler
    {
        private readonly IBindTaskToHotClientRequestAggregateService _bindTaskToHotClientRequestAggregateService;
        private readonly ITaskReadModel _taskReadModel;
        private readonly ICreateTaskAggregateService _createTaskAggregateService;
        private readonly IUpdateTaskAggregateService _updateTaskAggregateService;
        private readonly ICommonLog _logger;

        public ReplicateHotClientMessageAggregatedProcessingResultHandler(
            IBindTaskToHotClientRequestAggregateService bindTaskToHotClientRequestAggregateService,
            ITaskReadModel taskReadModel,
            ICreateTaskAggregateService createTaskAggregateService,
            IUpdateTaskAggregateService updateTaskAggregateService,
            ICommonLog logger)
        {
            _bindTaskToHotClientRequestAggregateService = bindTaskToHotClientRequestAggregateService;
            _taskReadModel = taskReadModel;
            _createTaskAggregateService = createTaskAggregateService;
            _updateTaskAggregateService = updateTaskAggregateService;
            _logger = logger;
        }

        public IEnumerable<KeyValuePair<Guid, MessageProcessingStageResult>> Handle(IEnumerable<KeyValuePair<Guid, List<IProcessingResultMessage>>> processingResultBuckets)
        {
            var handlingResults = new Dictionary<Guid, MessageProcessingStageResult>();
            var hotClientInfos = new Dictionary<long, HotClientInfo>();

            foreach (var processingResultBucket in processingResultBuckets)
            {
                foreach (var processingResults in processingResultBucket.Value)
                {
                    if (!Equals(processingResults.TargetFlow, FinalReplicateHotClientPerformedOperationsFlow.Instance))
                    {
                        continue;
                    }

                    var concreteProcessingResult = processingResults as ReplicateHotClientFinalProcessingResultsMessage;
                    if (concreteProcessingResult == null)
                    {
                        var messageProcessingStageResult = MessageProcessingStage.Handle
                                                                                 .EmptyResult()
                                                                                 .WithReport(string.Format("Unexpected processing result type {0} was achieved instead of {1}",
                                                                                                           processingResultBucket.Value.GetType().Name,
                                                                                                           typeof(ReplicateHotClientFinalProcessingResultsMessage).Name))
                                                                                 .AsFailed();
                        handlingResults.Add(processingResultBucket.Key, messageProcessingStageResult);

                        continue;
                    }

                    if (concreteProcessingResult.RequestEntity.TaskId != null)
                    {
                        _logger.WarnFormatEx(
                            "Hot client request with id {0} has been already processed. Task id {1} is not null. Skip the request processing.", 
                            concreteProcessingResult.RequestEntity.Id,
                            concreteProcessingResult.RequestEntity.TaskId);

                        handlingResults.Add(processingResultBucket.Key, MessageProcessingStage.Handle.EmptyResult().AsSucceeded());

                        continue;
                    }

                    HotClientInfo hotClientInfo;
                    if (!hotClientInfos.TryGetValue(concreteProcessingResult.RequestEntity.Id, out hotClientInfo))
                    {
                        hotClientInfo = new HotClientInfo { ReducedMessage = concreteProcessingResult };
                        hotClientInfos.Add(concreteProcessingResult.RequestEntity.Id, hotClientInfo);
                    }

                    hotClientInfo.OriginalMessageIds.Add(processingResultBucket.Key);
                }
            }

            foreach (var hotClientBucket in hotClientInfos)
            {
                var hotClientInfo = hotClientBucket.Value.ReducedMessage;

                MessageProcessingStageResult messageProcessingStageResult;

                try
                {
                    // TODO {all, 17.07.2014}: возможно стоит вынести процесс создания task в operationservice, т.к. после отказа от MsCRM бизнесс процесс останется, просто действия будут заводиться в ERM
                    var taskId = CreateTask(
                                    hotClientInfo.Owner, 
                                    hotClientInfo.HotClient, 
                                    hotClientInfo.RegardingObject);

                    _bindTaskToHotClientRequestAggregateService.BindWithCrmTask(hotClientInfo.RequestEntity, taskId);

                    messageProcessingStageResult = MessageProcessingStage.Handle
                                                                         .EmptyResult()
                                                                         .AsSucceeded();
                }
                catch (Exception ex)
                {
                    var msg = string.Format("Can't create hot client task for request with id = {0}", hotClientInfo.RequestEntity.Id);
                    _logger.ErrorFormatEx(ex, msg);
                    
                    messageProcessingStageResult = MessageProcessingStage.Handle
                                                                         .EmptyResult()
                                                                         .WithExceptions(ex)
                                                                         .WithReport(msg)
                                                                         .AsFailed();
                }

                foreach (var originalMessageId in hotClientBucket.Value.OriginalMessageIds)
                {
                    handlingResults.Add(originalMessageId, messageProcessingStageResult);
                }
            }

            return handlingResults;
        }

        private sealed class HotClientInfo
        {
            public HotClientInfo()
            {
                OriginalMessageIds = new HashSet<Guid>();
            }

            public ISet<Guid> OriginalMessageIds { get; private set; }
            public ReplicateHotClientFinalProcessingResultsMessage ReducedMessage { get; set; }
        }

        // TODO {all, 15.07.2014}: скорее всего код должен обладать свойством идемпотентности, т.е. если в CRM уже создали задачу, повторно создавать её аналог не стоит, вопрос как этого достичь
        private Guid CreateTask(UserDto owner, HotClientRequestDto hotClient, RegardingObject regardingObject)
        {
            try
            {
                var task = new Task
                    {
                        Header = BLResources.HotClientSubject,
                        Description = string.Format(BLResources.HotClientDescriptionTemplate, hotClient.ContactPhone, hotClient.ContactName),
                        ScheduledOn = hotClient.CreationDate,
                        TaskType = TaskType.WarmClient,
                        Status = ActivityStatus.InProgress,
                        Priority = ActivityPriority.Average,

                        OwnerCode = owner.Id,
                        IsActive = true,
                    };

                if (!string.IsNullOrWhiteSpace(hotClient.Description))
                {
                    task.Description += Environment.NewLine + hotClient.Description;
                }

                _createTaskAggregateService.Create(task);

                // В отношении
                if (regardingObject != null)
                {
                    _updateTaskAggregateService.ChangeRegardingObjects(task, Enumerable.Empty<TaskRegardingObject>(), new[] { 
                        new TaskRegardingObject
                        {
                            SourceEntityId = task.Id, 
                            TargetEntityName = regardingObject.EntityName, 
                            TargetEntityId = regardingObject.EntityId
                        } });
                }

                // TODO {all, 24.09.2014}: по-прежнему работаем с Guid'ами, т.к. HotClientRequest.TaskId того же типа, это надо будет менять синхронно после выхода ERM действий
                // только из-за этого мы должны заново прочитать задачу, так как ReplicationCode назначен при сохранении без синхронизации
                // как будет связь по task.Id - чтение надо убрать
                task = _taskReadModel.GetTask(task.Id);

                return task.ReplicationCode;
            }
            catch (Exception ex)
            {
                var message = string.Format("Ошибка при репликации сущности {0} с идентификатором {1}",
                                            "HotClientRequest",
                                            hotClient.Id);
                throw new IntegrationException(message, ex);
            }
        }
    }
}