using System.Collections;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    [DataContract]
    public sealed class EntityDtoListResult : ListResult, IDataListResult
    {
        public override ListResultType ResultType
        {
            get
            {
                return ListResultType.Dto;
            }

            set
            {
            }
        }

        IEnumerable IDataListResult.Data
        {
            get { return Data; }
        }

        [DataMember]
        public ICollection Data { get; set; }
    }
}