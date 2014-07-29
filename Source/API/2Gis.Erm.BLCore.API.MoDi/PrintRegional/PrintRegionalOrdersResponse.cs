using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.MoDi.PrintRegional
{
    [DataContract]
    public sealed class PrintRegionalOrdersResponse
    {
        [DataMember]
        public PrintRegionalOrdersResponseItem[] Items { get; set; }
    }
}