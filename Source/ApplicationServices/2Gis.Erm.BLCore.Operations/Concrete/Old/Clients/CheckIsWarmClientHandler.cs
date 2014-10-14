using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals.Settings;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Data.Services;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Clients
{
    public class CheckIsWarmClientHandler : RequestHandler<CheckIsWarmClientRequest, CheckIsWarmClientResponse>
    {
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IWarmClientProcessingSettings _warmClientProcessingSettings;
        private readonly IClientRepository _repository;

        public CheckIsWarmClientHandler(IMsCrmSettings msCrmSettings,
                                        IWarmClientProcessingSettings warmClientProcessingSettings,
                                        IClientRepository repository)
        {
            _msCrmSettings = msCrmSettings;
            _warmClientProcessingSettings = warmClientProcessingSettings;
            _repository = repository;
        }

        protected override CheckIsWarmClientResponse Handle(CheckIsWarmClientRequest request)
        {
            var negativeResponse = new CheckIsWarmClientResponse { IsWarmClient = false };

            if (_msCrmSettings.IntegrationMode == MsCrmIntegrationMode.Disabled)
            {
                return negativeResponse;
            }

            var clientInfo = _repository.GetClientReplicationData(request.ClientId);

            // Ищем закрытые задачи с типом "Тёплый клиент" у фирм клиента и у самого клиента.
            var checkTasks = clientInfo.FirmReplicationCodes
                                       .Select(code => new Task<CheckIsWarmClientResponse>(() => CheckFirm(_msCrmSettings.CreateDataContext(),
                                                                                                           code,
                                                                                                           _warmClientProcessingSettings.WarmClientDaysCount)))
                                       .Union(new[]
                                                  {
                                                      new Task<CheckIsWarmClientResponse>(() => CheckClient(_msCrmSettings.CreateDataContext(),
                                                                                                            clientInfo.ClientReplicationCode,
                                                                                                            _warmClientProcessingSettings.WarmClientDaysCount))
                                                  })
                                       .ToArray();

            try
            {
                foreach (var task in checkTasks)
                {
                    task.Start();
                }

                Task.WaitAll(checkTasks);

                var positiveResponse = checkTasks.Select(x => x.Result).FirstOrDefault(response => response != null && response.IsWarmClient);
                return positiveResponse ?? negativeResponse;
            }
            catch (InvalidOperationException ex)
            {
                throw new NotificationException(BLResources.Errors_DynamicsCrmConectionFailed, ex);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.All(exception => exception is WebException))
                {
                    throw new NotificationException(BLResources.Errors_DynamicsCrmConectionFailed, ex.InnerExceptions.First());
                }

                throw new BusinessLogicException(ex.Message, ex);
            }
        }

        private static CheckIsWarmClientResponse CheckFirm(ICrmDataContext crmDataContext, Guid firmReplicationCode, int warmClientDaysCount)
        {
            var firmObject = crmDataContext.GetFirm(firmReplicationCode);

            var existingFirmTask = FindRelatedWarmTask(firmObject, "dg_firm_Tasks", warmClientDaysCount);

            return existingFirmTask == null
                       ? null
                       : new CheckIsWarmClientResponse
                       {
                           IsWarmClient = true,
                           TaskActualEnd = existingFirmTask.ActualEnd,
                           TaskDescription = existingFirmTask.Description,
                       };
        }

        private static CheckIsWarmClientResponse CheckClient(ICrmDataContext crmDataContext, Guid clientReplicationCode, int warmClientDaysCount)
        {
            var clientObject = crmDataContext.GetClient(clientReplicationCode);
            var existingClosedTask = FindRelatedWarmTask(clientObject, "Account_Tasks", warmClientDaysCount);

            return existingClosedTask == null
                       ? null
                       : new CheckIsWarmClientResponse
                       {
                           IsWarmClient = true,
                           TaskActualEnd = existingClosedTask.ActualEnd,
                           TaskDescription = existingClosedTask.Description,
                       };
        }

        private static TaskSummary FindRelatedWarmTask(ICrmEntity crmObject, string taskRelationName, int days)
        {
            return crmObject.GetRelatedEntities(taskRelationName)
                            .Where(x => Convert.ToInt32(x["statuscode"].Value, CultureInfo.InvariantCulture) == 5 &&
                                        (DateTime.Now - x.GetPropertyValue<DateTime>("actualend")).TotalDays <= days &&
                                        Convert.ToInt32(x["dg_type"].Value, CultureInfo.InvariantCulture) == 1)
                            .Select(x => new TaskSummary
                                             {
                                                 ActualEnd = x.GetPropertyValue<DateTime>("actualend"),
                                                 Description = x.GetPropertyValue<string>("subject")
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
