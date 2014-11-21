using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases.Old
{
    public sealed class PrepareValidationReportRequest : Request
    {
        public TimePeriod Period { get; set; }
        public long OrganizationUnitId { get; set; }
        public IEnumerable<ReleaseProcessingMessage> ValidationResults { get; set; }
    }
}