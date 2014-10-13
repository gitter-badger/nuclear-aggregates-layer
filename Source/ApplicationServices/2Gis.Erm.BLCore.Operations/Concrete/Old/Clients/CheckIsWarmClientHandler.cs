using System;
using System.Globalization;
using System.Linq;
using System.Net;

using DoubleGis.Erm.BLCore.API.Aggregates.Clients;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals.Settings;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

using Microsoft.Xrm.Client;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Clients
{
    public class CheckIsWarmClientHandler : RequestHandler<CheckIsWarmClientRequest, CheckIsWarmClientResponse>
    {
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IWarmClientProcessingSettings _warmClientProcessingSettings;
        private readonly IClientRepository _repository;

        public CheckIsWarmClientHandler(
            IMsCrmSettings msCrmSettings,
            IWarmClientProcessingSettings warmClientProcessingSettings,
            IClientRepository repository)
        {
            _msCrmSettings = msCrmSettings;
            _warmClientProcessingSettings = warmClientProcessingSettings;
            _repository = repository;
        }

        protected override CheckIsWarmClientResponse Handle(CheckIsWarmClientRequest request)
        {
            var response = new CheckIsWarmClientResponse();
            if (_msCrmSettings.EnableReplication)
            {
                var clientInfo = _repository.GetClientReplicationData(request.ClientId);

                try
                {
                    var crmDataContext = _msCrmSettings.CreateDataContext();
                    // Сначала ищем закрытые задачи с типом "Тёплый клиент" у фирм клиента, 
                    // потом у самого клиента.
                    var positiveResponseForFirm =
                        clientInfo.FirmReplicationCodes
                                  .AsParallel()
                                  .Select(firmReplicationCode =>
                                              {
                                                  var firmObject = crmDataContext.GetFirm(firmReplicationCode);

                                                  var existingFirmTask = FindRelatedWarmTask(firmObject, "dg_firm_Tasks", _warmClientProcessingSettings.WarmClientDaysCount);

                                                  if (existingFirmTask != null)
                                                  {
                                                      response.IsWarmClient = true;
                                                      response.TaskActualEnd = existingFirmTask.ActualEnd;
                                                      response.TaskDescription = existingFirmTask.Description;
                                                      return response;
                                                  }

                                                  return null;
                                              })
                                  .AsEnumerable()
                                  .FirstOrDefault(clientResponse => clientResponse != null);
                    
                    if (positiveResponseForFirm != null)
                    {
                        return positiveResponseForFirm;
                    }

                    var clientObject = crmDataContext.GetClient(clientInfo.ClientReplicationCode);
                    var existingClosedTask = FindRelatedWarmTask(clientObject, "Account_Tasks", _warmClientProcessingSettings.WarmClientDaysCount);

                    if (existingClosedTask != null)
                    {
                        response.IsWarmClient = true;
                        response.TaskActualEnd = existingClosedTask.ActualEnd;
                        response.TaskDescription = existingClosedTask.Description;
                    }
                }
                catch (WebException ex)
                {
                    throw new NotificationException(BLResources.Errors_DynamicsCrmConectionFailed, ex);
                }
            }

            return response;
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
                                 }).FirstOrDefault();
        }

        private class TaskSummary
        {
            public DateTime ActualEnd { get; set; }
            public string Description { get; set; }
        }
    }
}
