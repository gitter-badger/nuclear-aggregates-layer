﻿using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Disqualify;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Clients
{
    // TODO {v.lapeev, 11.02.2014}: кандидат на MultiCulture
    // DONE {y.baranihin, 26.03.2014}: поддерживаю
    public class MultiCultureCheckClientActivitiesHandler : RequestHandler<CheckClientActivitiesRequest, EmptyResponse>, IChileAdapted, ICyprusAdapted, ICzechAdapted, IUkraineAdapted
    {
        private readonly IActivityReadModel _activityReadModel;

        public MultiCultureCheckClientActivitiesHandler(IActivityReadModel activityReadModel)
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
