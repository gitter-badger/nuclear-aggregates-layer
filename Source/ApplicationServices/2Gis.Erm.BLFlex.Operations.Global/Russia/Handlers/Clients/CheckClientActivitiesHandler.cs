using System.Linq;
using System.Net;

using DoubleGis.Erm.BL.Aggregates.Firms;
using DoubleGis.Erm.BL.API.Operations.Remote.Disqualify;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Operations.Operations.Concrete.Old.Clients;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Data.Services;
using Microsoft.Xrm.Client.Services;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Handlers.Clients
{
    public sealed class CheckClientActivitiesHandler : RequestHandler<CheckClientActivitiesRequest, EmptyResponse>, IRussiaAdapted
    {
        private readonly IMsCrmSettings _crmSettings;
        private readonly IFinder _finder;

        public CheckClientActivitiesHandler(IMsCrmSettings crmSettings, IFinder finder)
        {
            _crmSettings = crmSettings;
            _finder = finder;
        }

        protected override EmptyResponse Handle(CheckClientActivitiesRequest request)
        {
            // Проверяем открытые связанные объекты:
            // Проверяем наличие открытых Действий (Звонок, Встреча, Задача и пр.), связанных с данным Клиентом и его фирмами, 
            // если есть открытые Действия, выдается сообщение "Необходимо закрыть все активные действия с данным Клиентом и его фирмами".
            if (_crmSettings.EnableReplication)
            {
                var clientReplicationCode = _finder.Find(GenericSpecifications.ById<Client>(request.ClientId)).Select(x => x.ReplicationCode).Single();

                try
                {
                    var crmConnection = new CrmConnection("CrmConnection");
                    var crmDataContext = new CrmDataContext(null, () => new OrganizationService(null, crmConnection));

                    if (ActivityHelper.HasAnyOpenedActivities(crmDataContext, clientReplicationCode))
                    {
                        throw new NotificationException(BLResources.NeedToCloseAllActivities);
                    }
                        
                    var firmReplicationCodes = _finder.Find(FirmSpecifications.Find.FirmsByClientId(request.ClientId))
                        .Select(x => x.ReplicationCode).ToArray();

                    if (firmReplicationCodes.Any(x => ActivityHelper.HasAnyOpenedActivities(crmDataContext, x)))
                    {
                        throw new NotificationException(BLResources.NeedToCloseAllActivities);
                    }
                }
                catch (WebException ex)
                {
                    throw new NotificationException(BLResources.Errors_DynamicsCrmConectionFailed, ex);
                }
            }

            return Response.Empty;
        }
    }
}