using System.IO;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Print
{
    public sealed class PrintFormDocument
    {
        public Stream Stream { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
