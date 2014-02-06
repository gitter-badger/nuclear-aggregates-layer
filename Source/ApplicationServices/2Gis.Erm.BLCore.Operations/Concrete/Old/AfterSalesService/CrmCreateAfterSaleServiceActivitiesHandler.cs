using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using DoubleGis.Erm.BLCore.Aggregates.Deals;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AfterSaleServices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Xrm.Client.Data.Services;
using Microsoft.Xrm.Client.Services;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.AfterSalesService
{
    public class CrmCreateAfterSaleServiceActivitiesHandler 
        : RequestHandler<CrmCreateAfterSaleServiceActivitiesRequest, CrmCreateAfterSaleServiceActivitiesResponse>
    {
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IFinder _finder;
        private readonly IMsCrmInteractionService _crmDynamicsInteractionService;

        public CrmCreateAfterSaleServiceActivitiesHandler(
            IMsCrmSettings msCrmSettings, 
            IFinder finder, 
            IMsCrmInteractionService crmDynamicsInteractionService)
        {
            _msCrmSettings = msCrmSettings;
            _finder = finder;
            _crmDynamicsInteractionService = crmDynamicsInteractionService;
        }

        protected override CrmCreateAfterSaleServiceActivitiesResponse Handle(CrmCreateAfterSaleServiceActivitiesRequest request)
        {
            var createdPhonecallsCount = 0;
            var errorCount = 0;
            var errorLog = new StringBuilder();
            if (_msCrmSettings.EnableReplication && request.CreatedActivities.Any())
            {
                var dealsIds = request.CreatedActivities.Select(a => a.DealId).Distinct();
                var dealsMapping = _finder.Find<Deal>(d => dealsIds.Contains(d.Id))
                    .Select(d => new
                        {
                            DealId = d.Id,
                            DealReplicationCode = d.ReplicationCode,
                            ClientReplicationCode = d.Client.ReplicationCode,
                            ClientName = d.Client.Name,
                            d.OwnerCode
                        })
                    .ToDictionary(x => x.DealId, x => x);

                try
                {
                    // Создание звонков производим от пользователя "Integration"
                    var crmConnection = _msCrmSettings.CreateConnection();
                    var crmDataContext = new CrmDataContext(null, () => new OrganizationService(null, crmConnection));
                    var integrationUserId = (Guid)crmDataContext.GetEntities(EntityName.organization.ToString()).Select(x => x["integrationuserid"].Value).First();
                    crmConnection.ImpersonatedUser = integrationUserId;

                    // Crm-ные данные куратора для каждой сделки.
                    Dictionary<long, CrmUserDto> dealToOwnerUserMapping;
                    {
                        var dealOwners = dealsMapping.Select(x => x.Value.OwnerCode).Distinct().ToArray();
                        var usersErmMapping = _finder.Find<User>(x => dealOwners.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Account);
                        var usersCrmMapping = _crmDynamicsInteractionService.GetUserMappings(usersErmMapping);

                        dealToOwnerUserMapping = dealsMapping.ToDictionary(x => x.Key, x => usersCrmMapping[x.Value.OwnerCode]);
                    }

                    foreach (var activity in request.CreatedActivities)
                    {
                        try
                        {
                            var dealDetails = dealsMapping[activity.DealId];
                            var userDto = dealToOwnerUserMapping[activity.DealId];
                            CreatePhonecall(crmDataContext, activity, dealDetails.DealReplicationCode, dealDetails.ClientReplicationCode, dealDetails.ClientName, userDto);
                            createdPhonecallsCount++;
                        }
                        catch (WebException ex)
                        {
                            if (ex.Response == null)
                            {
                                continue;
                            }

                                var responseStream = ex.Response.GetResponseStream();
                            if (responseStream == null)
                                {
                                continue;
                            }

                            var responseXml = new StreamReader(responseStream).ReadToEnd();
                                    errorLog.AppendFormat(BLResources.AfterSalesServiceCreationErrorFormat, activity.Id, responseXml, Environment.NewLine);
                                    errorCount++;
                                }
                        catch (ApplicationException aex)
                        {
                            errorLog.AppendFormat(BLResources.AfterSalesServiceCreationErrorFormat, activity.Id, aex.Message, Environment.NewLine);
                            errorCount++;
                        }
                    }

                    crmDataContext.SaveChanges();
                }
                catch (WebException ex)
                {
                    throw new NotificationException(BLResources.Errors_DynamicsCrmConectionFailed, ex);
                }
            }

            var result = new CrmCreateAfterSaleServiceActivitiesResponse
            {
                    CreatedPhonecallsCount = createdPhonecallsCount, 
                    ErrorCount = errorCount
                };
            if (errorLog.Length > 0)
            {
                result.ErrorLog = errorLog.ToString();
            }

            return result;
        }

        private static void CreatePhonecall(CrmDataContext crmDataContext, 
            AfterSaleServiceActivity activity, 
            Guid dealReplicationCode,
            Guid clientReplicationCode,
            string clientName,
            CrmUserDto userDto)
        {
            // Вычисляем время в UTC таким образом, что бы для каждого пользователя оно отображалось как 10:00.
            var activityDay = GetActivityDay(activity);
            var activityEndTime = activityDay + TimeSpan.FromHours(10.0) + userDto.TimeZoneTotalBias;
            var activityStartTime = activityEndTime - TimeSpan.FromMinutes(30.0);

            var entity = crmDataContext.CreateEntity(EntityName.phonecall.ToString());

            entity.SetPropertyValue("scheduledstart", FromUser(activityStartTime));
            entity.SetPropertyValue("scheduledend", FromUser(activityEndTime));
            entity.SetPropertyValue("actualdurationminutes", 30);
            entity.SetPropertyValue("scheduleddurationminutes", 30);

            entity.SetPropertyValue("dg_purpose", GetPhonecallPurpose(activity));
            entity.SetPropertyValue("subject", GetActivitySubject(activity, clientName));
            entity.SetPropertyValue("dg_aftersaletype", new Picklist(activity.AfterSaleServiceType));
            entity.SetPropertyValue("directioncode", true);

            entity.SetPropertyValue("createdon", CrmDateTime.Now);
            entity.SetPropertyValue("modifiedon", CrmDateTime.Now);

            // "To" field
            var toParty = new DynamicEntity("activityparty");
            toParty.Properties.Add(new LookupProperty("partyid", new Lookup(EntityName.account.ToString(), clientReplicationCode)));
            entity.SetPropertyValue("to", new[] { toParty });

            // "From" field
            var fromParty = new DynamicEntity("activityparty");
            fromParty.Properties.Add(new LookupProperty("partyid", new Lookup(EntityName.systemuser.ToString(), userDto.CrmUserId)));
            entity.SetPropertyValue("from", new[] { fromParty });

            entity.SetPropertyValue("ownerid", new Owner(EntityName.systemuser.ToString(), userDto.CrmUserId));

            // Сделка
            entity.SetPropertyValue("regardingobjectid", new Lookup(EntityName.opportunity.ToString(), dealReplicationCode));

            crmDataContext.AddObject(EntityName.phonecall.ToString(), entity);
        }

        private static Picklist GetPhonecallPurpose(AfterSaleServiceActivity activity)
        {
            switch ((AfterSaleServiceType)activity.AfterSaleServiceType)
            {
                case AfterSaleServiceType.ASS1: 
                case AfterSaleServiceType.ASS2:
                case AfterSaleServiceType.ASS3:
                    return new Picklist((int)ActivityPurpose.Service);
                case AfterSaleServiceType.ASS4:
                    return new Picklist((int)ActivityPurpose.Prolongation);

                default: throw new InvalidOperationException("Unexpected value for activity: " + activity);
            }
        }

        private static string GetActivitySubject(AfterSaleServiceActivity activity, string clientName)
        {
            return string.Format("{0} {1}", clientName, ((AfterSaleServiceType) activity.AfterSaleServiceType).ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
        }

        private static DateTime GetActivityDay(AfterSaleServiceActivity activity)
        {
            var month = ReleaseNumberUtils.AbsoluteReleaseNumberToMonth(activity.AbsoluteMonthNumber);

            switch ((AfterSaleServiceType)activity.AfterSaleServiceType)
            {
                case AfterSaleServiceType.ASS1: return new DateTime(month.Year, month.Month, 5, 0, 0, 0, DateTimeKind.Utc);
                case AfterSaleServiceType.ASS2: return new DateTime(month.Year, month.Month, 15, 0, 0, 0, DateTimeKind.Utc);
                case AfterSaleServiceType.ASS3: return new DateTime(month.Year, month.Month, 20, 0, 0, 0, DateTimeKind.Utc);
                case AfterSaleServiceType.ASS4: return new DateTime(month.Year, month.Month, 10, 0, 0, 0, DateTimeKind.Utc);

                default: throw new InvalidOperationException("Unexpected value for activity: " + activity);
            }
        }

        /// <summary>
        /// Workaround для конвертации CrmDateTime. - 
        /// CrmDateTime.FromUser() кидает InvalidOperationException.
        /// </summary>
        private static CrmDateTime FromUser(DateTime userTime)
        {
            return new CrmDateTime(userTime.ToString("yyyy/MM/ddTHH:mm:ssK"));
        }
    }
}
