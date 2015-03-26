using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.Dial
{
    [DataContract]
    public class DialResult
    {
        public DialResult(string result)
        {
            Result = result;
        }

        [DataMember]
        public string Result { get; set; }
    }
}
