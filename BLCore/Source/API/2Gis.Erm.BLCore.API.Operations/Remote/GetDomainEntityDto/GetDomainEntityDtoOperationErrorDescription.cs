using System.Runtime.Serialization;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Remote.GetDomainEntityDto
{
    [DataContract]
    public class GetDomainEntityDtoOperationErrorDescription : IBasicOperationErrorDescription
    {
        public GetDomainEntityDtoOperationErrorDescription(IEntityType entityName, string message)
        {
            EntityName = entityName;
            Message = message;
        }

        [DataMember]
        public IEntityType EntityName { get; private set; }
        [DataMember]
        public string Message { get; private set; }
    }
}