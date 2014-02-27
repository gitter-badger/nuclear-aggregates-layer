using System.Linq;
using System.Net;

using DoubleGis.Erm.BLCore.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Disqualify;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Clients;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Clients
{
    public sealed class CheckClientActivitiesHandler : RequestHandler<CheckClientActivitiesRequest, EmptyResponse>, IRussiaAdapted
    {
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IFinder _finder;

        public CheckClientActivitiesHandler(IMsCrmSettings msCrmSettings, IFinder finder)
        {
            _msCrmSettings = msCrmSettings;
            _finder = finder;
        }

        protected override EmptyResponse Handle(CheckClientActivitiesRequest request)
        {
            // Проверяем открытые связанные объекты:
            // Проверяем наличие открытых Действий (Звонок, Встреча, Задача и пр.), связанных с данным Клиентом и его фирмами, 
            // если есть открытые Действия, выдается сообщение "Необходимо закрыть все активные действия с данным Клиентом и его фирмами".
            if (_msCrmSettings.EnableReplication)
            {
                var clientReplicationCode = _finder.Find(Specs.Find.ById<Client>(request.ClientId)).Select(x => x.ReplicationCode).Single();

                try
                {
                    var crmDataContext = _msCrmSettings.CreateDataContext();
                    if (ActivityHelper.HasAnyOpenedActivities(crmDataContext, clientReplicationCode))
                    {
                        throw new NotificationException(BLResources.NeedToCloseAllActivities);
                    }
                        
                    var firmReplicationCodes = _finder.Find(FirmSpecs.Firms.Find.ByClient(request.ClientId))
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