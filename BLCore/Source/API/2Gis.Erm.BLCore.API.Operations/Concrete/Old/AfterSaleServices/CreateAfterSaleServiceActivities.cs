using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AfterSaleServices
{
    public sealed class CreateAfterSaleServiceActivitiesRequest : Request
    {
        public CreateAfterSaleServiceActivitiesRequest(long organizationUnitId, TimePeriod month)
        {
            OrganizationUnitId = organizationUnitId;
            Month = month;
        }

        public long OrganizationUnitId { get; private set; }
        public TimePeriod Month { get; private set; }
    }

    public sealed class CreateAfterSaleServiceActivitiesResponse : Response
    {
        public CreateAfterSaleServiceActivitiesResponse(IEnumerable<AfterSaleServiceActivity> createdActivities, int dealsProcessed)
        {
            CreatedActivities = createdActivities;
            DealsProcessed = dealsProcessed;
        }

        public IEnumerable<AfterSaleServiceActivity> CreatedActivities { get; private set; }
        public int DealsProcessed { get; private set; }
    }
}
