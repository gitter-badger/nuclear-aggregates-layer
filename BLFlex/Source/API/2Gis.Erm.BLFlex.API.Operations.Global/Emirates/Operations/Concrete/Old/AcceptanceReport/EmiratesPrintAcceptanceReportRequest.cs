using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Old.AcceptanceReport
{
    public sealed class EmiratesPrintAcceptanceReportRequest : Request, IEmiratesAdapted
    {
        public long OrderId { get; set; }
    }
}