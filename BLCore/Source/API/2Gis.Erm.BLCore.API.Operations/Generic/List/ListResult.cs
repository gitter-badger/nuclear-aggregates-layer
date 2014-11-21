using System.Collections;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    public interface IDataListResult
    {
        IEnumerable Data { get; }
    }

    [DataContract]
    public abstract class ListResult
    {
        [DataMember]
        public abstract ListResultType ResultType { get; set; }

        [DataMember]
        public int RowCount { get; set; }

        [DataMember]
        public string MainAttribute { get; set; }
    }
}