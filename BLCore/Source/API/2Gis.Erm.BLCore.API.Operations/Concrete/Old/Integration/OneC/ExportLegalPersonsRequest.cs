using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC
{
    public sealed class ExportLegalPersonsRequest : Request
    {
        public long? OrganizationUnitId { get; set; }
        public DateTime PeriodStart { get; set; }
    }
}