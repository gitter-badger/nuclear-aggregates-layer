using System;
using System.Linq;
using System.Net;

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

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Firms
{
    public sealed class CheckMsCrmFirmActivitiesRequest : Request
    {
        public long Id { get; set; }
    }

    // called when disqualifying client
    public sealed class CheckMsCrmFirmActivitiesHandler : RequestHandler<CheckMsCrmFirmActivitiesRequest, EmptyResponse>
    {
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly IFinder _finder;

        public CheckMsCrmFirmActivitiesHandler(IMsCrmSettings msCrmSettings, IFinder finder)
        {
            _msCrmSettings = msCrmSettings;
            _finder = finder;
        }

        protected override EmptyResponse Handle(CheckMsCrmFirmActivitiesRequest request)
        {
            // Проверяем открытые связанные объекты:
            // Шаг 2. Проверяем наличие открытых Действий (Звонок, Встреча, Задача и пр.), связанных с данной Фирмой, 
            // если есть открытые Действия, выдается сообщение "Необходимо закрыть все активные действия с данной Фирмой".
            if (_msCrmSettings.EnableReplication)
            {
                Guid firmReplicationCode = _finder.Find(Specs.Find.ById<Firm>(request.Id)).Select(firm => firm.ReplicationCode).Single();

                try
                {
                    var crmDataContext = _msCrmSettings.CreateDataContext();

                    if (ActivityHelper.HasAnyOpenedActivities(crmDataContext, firmReplicationCode))
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
