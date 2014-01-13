using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages
{
    public sealed class SaveLocalMessageRequest : Request
    {
        public long[] Ids { get; set; }
    }
}