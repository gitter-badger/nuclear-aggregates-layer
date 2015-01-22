using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals.Settings;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Clients
{
    public class CheckIsWarmClientHandler : RequestHandler<CheckIsWarmClientRequest, CheckIsWarmClientResponse>
    {
        private readonly IWarmClientProcessingSettings _warmClientProcessingSettings;
        private readonly IClientReadModel _clientReadModel;
        private readonly ITaskReadModel _taskReadModel;

        public CheckIsWarmClientHandler(
            IClientReadModel clientReadModel,
            ITaskReadModel taskReadModel,
            IWarmClientProcessingSettings warmClientProcessingSettings)
        {
            _clientReadModel = clientReadModel;
            _taskReadModel = taskReadModel;
            _warmClientProcessingSettings = warmClientProcessingSettings;
        }

        protected override CheckIsWarmClientResponse Handle(CheckIsWarmClientRequest request)
        {
            var response = new CheckIsWarmClientResponse();

            var client = _clientReadModel.GetClient(request.ClientId);
            if (client == null)
            {
                throw new NotificationException("The client does not exist.");
            }

            // Сначала ищем закрытые задачи с типом "Тёплый клиент" у фирм клиента, 
            // потом у самого клиента.
            foreach (var firmId in client.Firms.Select(x => x.Id))
            {
                var existingFirmTask = FindRelatedWarmTask(EntityType.Instance.Firm(), firmId);
                if (existingFirmTask != null)
                {
                    response.IsWarmClient = true;
                    response.TaskActualEnd = existingFirmTask.ActualEnd;
                    response.TaskDescription = existingFirmTask.Description;
                    return response;
                }
            }

            var existingClosedTask = FindRelatedWarmTask(EntityType.Instance.Client(), client.Id);
            if (existingClosedTask != null)
            {
                response.IsWarmClient = true;
                response.TaskActualEnd = existingClosedTask.ActualEnd;
                response.TaskDescription = existingClosedTask.Description;
            }

            return response;
        }

        private TaskSummary FindRelatedWarmTask(IEntityType entityName, long entityId)
        {
            var days = _warmClientProcessingSettings.WarmClientDaysCount;

            return _taskReadModel.LookupTasksRegarding(entityName, entityId)
                                 .Where(x => x.Status == ActivityStatus.Completed &&
                                             x.TaskType == TaskType.WarmClient &&
                                             (DateTime.Now - x.ModifiedOn.Value).TotalDays <= days)
                                 .Select(x => new TaskSummary
                                                  {
                                                      ActualEnd = x.ModifiedOn.Value,
                                                      Description = x.Header
                                                  })
                                 .FirstOrDefault();
        }

        private class TaskSummary
        {
            public DateTime ActualEnd { get; set; }
            public string Description { get; set; }
        }
    }
}
