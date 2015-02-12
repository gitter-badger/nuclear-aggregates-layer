using System.Runtime.Serialization;

using DoubleGis.Erm.BLCore.API.Operations.Remote;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.Remote.List
{
    [DataContract]
    public class ListOperationErrorDescription : IBasicOperationErrorDescription
    {
        public ListOperationErrorDescription(IEntityType entityName, string message)
        {
            EntityName = entityName.Description;
            Message = message;
        }

        [DataMember]
        public string EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
