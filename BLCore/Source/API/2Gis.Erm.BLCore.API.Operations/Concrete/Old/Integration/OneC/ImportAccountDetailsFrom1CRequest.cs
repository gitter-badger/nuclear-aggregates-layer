using System.IO;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC
{
    public sealed class ImportAccountDetailsFrom1CRequest : ImportRequest
    {
        public string FileName { get; set; }
        public Stream InputStream { get; set; }
    }
}