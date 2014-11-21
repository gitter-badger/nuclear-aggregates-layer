using System.Collections.Generic;
using System.IO;

namespace DoubleGis.Erm.Platform.Common.PrintFormEngine
{
    public interface IPrintFormService
    {
        void PrintToDocx(Stream stream, object data, int currencyIsoCode);
        Stream MergeDocuments(IEnumerable<Stream> documents);
    }
}