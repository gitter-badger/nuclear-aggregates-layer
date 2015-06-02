using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.HotClients
{
    public class ProcessHotClientRequestOperationService : IProcessHotClientRequestOperationService
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IFirmReadModel _firmReadModel;

        private readonly ICreateTaskAggregateService _createTaskAggregateService;
        private readonly IUpdateTaskAggregateService _updateTaskAggregateService;
        private readonly IBindTaskToHotClientRequestAggregateService _bindTaskToHotClientRequestAggregateService;

        public ProcessHotClientRequestOperationService(
            IFirmReadModel firmReadModel,
            ICreateTaskAggregateService createTaskAggregateService,
            IUpdateTaskAggregateService updateUpdateTaskAggregateService,
            IBindTaskToHotClientRequestAggregateService bindTaskToHotClientRequestAggregateService,
            IOperationScopeFactory scopeFactory)
        {
            _firmReadModel = firmReadModel;
            _createTaskAggregateService = createTaskAggregateService;
            _updateTaskAggregateService = updateUpdateTaskAggregateService;
            _bindTaskToHotClientRequestAggregateService = bindTaskToHotClientRequestAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void CreateHotClientTask(HotClientRequestDto hotClientRequest, long ownerId, RegardingObject regardingObject)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ProcessHotClientRequestIdentity>())
            {
                var task = CreateTask(hotClientRequest, ownerId);
                AttachRegardingObject(task, regardingObject);

                BindTask(hotClientRequest.Id, task.Id);

                scope.Complete();
            }
        }

        private Task CreateTask(HotClientRequestDto hotClient, long ownerId)
        {
            // TODO {all, 24.09.2014}: выглядит подозрительно хранение в Header и Description уже локализованных строк
            var task = new Task
            {
                Header = BLResources.HotClientSubject,
                Description = DescribeTask(hotClient),
                ScheduledOn = hotClient.CreationDate,
                TaskType = TaskType.WarmClient,
                Status = ActivityStatus.InProgress,
                Priority = ActivityPriority.Average,
                OwnerCode = ownerId,
                
                IsActive = true,
            };

            _createTaskAggregateService.Create(task);

            return task;
        }

        private void AttachRegardingObject(Task task, RegardingObject regardingObject)
        {
            if (regardingObject == null)
            {
                return;
            }

            _updateTaskAggregateService.ChangeRegardingObjects(task,
                                                               Enumerable.Empty<TaskRegardingObject>(),
                                                               new[]
                                                                   {
                                                                       new TaskRegardingObject
                                                                           {
                                                                               SourceEntityId = task.Id,
                                                                               TargetEntityTypeId = regardingObject.EntityName.Id,
                                                                               TargetEntityId = regardingObject.EntityId
                                                                           }
                                                                   });
        }

        private void BindTask(long requestId, long taskId)
        {
            var hotClientRequest = _firmReadModel.GetHotClientRequest(requestId);
            _bindTaskToHotClientRequestAggregateService.BindTask(hotClientRequest, taskId);
        }

        private static string DescribeTask(HotClientRequestDto hotClient)
        {
            var description = string.Format(BLResources.HotClientDescriptionTemplate, hotClient.ContactPhone, hotClient.ContactName);

            if (!string.IsNullOrWhiteSpace(hotClient.Description))
            {
                description += Environment.NewLine + hotClient.Description;
            }

            return description;
        }
    }
}