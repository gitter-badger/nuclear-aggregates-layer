using DoubleGis.Erm.BL.Aggregates.Activities;
using DoubleGis.Erm.BL.API.Operations.Remote.Disqualify;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BL.Handlers.Clients
{
    public sealed class CzechCheckClientActivitiesHandler : RequestHandler<CheckClientActivitiesRequest, EmptyResponse>, ICzechAdapted
    {
        private readonly IActivityService _activityService;

        public CzechCheckClientActivitiesHandler(IActivityService activityService)
        {
            _activityService = activityService;
        }

        protected override EmptyResponse Handle(CheckClientActivitiesRequest request)
        {
            // ��������� �������� ��������� �������:
            // ��������� ������� �������� �������� (������, �������, ������ � ��.), ��������� � ������ �������� � ��� �������, 
            // ���� ���� �������� ��������, �������� ��������� "���������� ������� ��� �������� �������� � ������ �������� � ��� �������".
            var hasRelatedOpenedActivities = _activityService.CheckIfExistsRelatedActivities(request.ClientId);

            if (hasRelatedOpenedActivities)
            {
                throw new NotificationException(BLResources.NeedToCloseAllActivities);
            }

            return Response.Empty;
        }
    }
}