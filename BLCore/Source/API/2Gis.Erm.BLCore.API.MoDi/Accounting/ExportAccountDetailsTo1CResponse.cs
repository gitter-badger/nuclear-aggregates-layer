using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.MoDi.Accounting
{
    [DataContract]
    public class ExportAccountDetailsTo1CResponse
    {
        [DataMember]
        public FileDescription File { get; set; }
        [DataMember]
        public FileDescription ErrorFile { get; set; }
        [DataMember]
        public int ProcessedWithoutErrors { get; set; }
        [DataMember]
        public int BlockingErrorsAmount { get; set; }
        [DataMember]
        public int NonBlockingErrorsAmount { get; set; }
    }
}