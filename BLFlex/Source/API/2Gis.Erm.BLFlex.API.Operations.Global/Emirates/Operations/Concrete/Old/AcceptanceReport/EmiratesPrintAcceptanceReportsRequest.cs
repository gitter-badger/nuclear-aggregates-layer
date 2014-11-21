using System;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Old.AcceptanceReport
{
    public sealed class EmiratesPrintAcceptanceReportsRequest : Request, IEmiratesAdapted
    {
        public DateTime Month { get; set; }
        public long OrganizationUnitId { get; set; }
    }
}