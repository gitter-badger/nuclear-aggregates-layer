using DoubleGis.Erm.BL.Aggregates.Activities;
using DoubleGis.Erm.BL.API.Operations.Remote.Disqualify;
using DoubleGis.Erm.BL.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLFlex.Operations.Global.DuplicatesFromOperations
{
    // FIXME {all, 06.11.2013}: �������� �� BL.Operations - ��� ����� � ������ �������, ������ �� ������������ ������ � TFS ��-�� �������������� merge - ���� ��������� ��� �����, ��� RI �� 1.0 ����� �������� �������� ����� ������� ���� ���������� �� 2��
    // ������ ����������� ������� internal, ����� �� ������������� massprocessor
    internal sealed class CzechCheckClientActivitiesHandler : RequestHandler<CheckClientActivitiesRequest, EmptyResponse>, ICzechAdapted
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