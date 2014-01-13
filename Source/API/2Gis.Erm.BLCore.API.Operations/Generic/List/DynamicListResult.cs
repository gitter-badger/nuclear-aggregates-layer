using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    [DataContract]
    public sealed class DynamicListResult : ListResult, IDataListResult
    {
        public override ListResultType ResultType
        {
            get
            {
                return ListResultType.Dynamic;
            }

            set
            {
            }
        }

        [DataMember]
        public IEnumerable<DynamicListRow> Data { get; set; }

        IEnumerable IDataListResult.Data
        {
            get
            {
                return Data;
            }
        }
    }
}