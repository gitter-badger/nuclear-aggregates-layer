using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms
{
    public sealed class MakeRegionalAdsDocsRequest : Request
    {
        public DateTime StartPeriodDate { get; set; }
        public long SourceOrganizationUnitId { get; set; }
    }
}
