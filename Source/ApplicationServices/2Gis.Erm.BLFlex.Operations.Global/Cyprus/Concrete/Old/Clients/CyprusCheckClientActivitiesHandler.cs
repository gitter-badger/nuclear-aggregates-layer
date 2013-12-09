using DoubleGis.Erm.BL.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BL.API.Operations.Remote.Disqualify;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Concrete.Old.Clients
{
    public sealed class CyprusCheckClientActivitiesHandler : RequestHandler<CheckClientActivitiesRequest, EmptyResponse>, ICyprusAdapted
    {
        private readonly IActivityReadModel _activityReadModel;

        public CyprusCheckClientActivitiesHandler(IActivityReadModel activityReadModel)
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