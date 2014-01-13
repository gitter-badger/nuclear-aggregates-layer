using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    [DataContract]
    public class DynamicPropertyValue
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public object Value { get; set; }
    }
}
