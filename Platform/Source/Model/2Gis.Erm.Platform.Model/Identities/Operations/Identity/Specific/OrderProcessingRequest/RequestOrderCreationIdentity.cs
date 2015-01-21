using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest
{
    [DataContract]
    public sealed class RequestOrderCreationIdentity : OperationIdentityBase<RequestOrderCreationIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.RequestOrderCreationIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Create OrderProcessingRequest to create new order";
            }
        }
    }
}
