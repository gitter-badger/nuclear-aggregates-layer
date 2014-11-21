using System.IO;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common
{
    public class StreamResponse : Response
    {
        public Stream Stream { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}