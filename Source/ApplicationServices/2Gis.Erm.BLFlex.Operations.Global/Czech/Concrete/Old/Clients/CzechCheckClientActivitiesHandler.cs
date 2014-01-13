using DoubleGis.Erm.BLCore.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Disqualify;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Concrete.Old.Clients
{
    public sealed class CzechCheckClientActivitiesHandler : RequestHandler<CheckClientActivitiesRequest, EmptyResponse>, ICzechAdapted
    {
        private readonly IActivityReadModel _activityReadModel;

        public CzechCheckClientActivitiesHandler(IActivityReadModel activityReadModel)
        {
            _activityReadModel = activityReadModel;
        }

        protected override EmptyResponse Handle(CheckClientActivitiesRequest request)
        {
            // Проверяем открытые связанные объекты:
            // Проверяем наличие открытых Действий (Звонок, Встреча, Задача и пр.), связанных с данным Клиентом и его фирмами, 
            // если есть открытые Действия, выдается сообщение "Необходимо закрыть все активные действия с данным Клиентом и его фирмами".
            var hasRelatedOpenedActivities = _activityReadModel.CheckIfRelatedActivitiesExists(request.ClientId);

            if (hasRelatedOpenedActivities)
            {
                throw new NotificationException(BLResources.NeedToCloseAllActivities);
            }

            return Response.Empty;
        }
    }
}