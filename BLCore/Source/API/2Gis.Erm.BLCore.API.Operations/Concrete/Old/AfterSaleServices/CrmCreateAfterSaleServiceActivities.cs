using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AfterSaleServices
{
    public class CrmCreateAfterSaleServiceActivitiesRequest : Request
    {
        public CrmCreateAfterSaleServiceActivitiesRequest(IEnumerable<AfterSaleServiceActivity> createdActivities)
        {
            CreatedActivities = createdActivities;
        }

        public IEnumerable<AfterSaleServiceActivity> CreatedActivities { get; private set; }
    }

    public class CrmCreateAfterSaleServiceActivitiesResponse : Response
    {
        public int CreatedPhonecallsCount { get; set; }
        public int ErrorCount { get; set; }
        public string ErrorLog { get; set; }
    }
}
