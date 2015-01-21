using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest
{
    [DataContract]
    public sealed class RequestOrderProlongationIdentity : OperationIdentityBase<RequestOrderProlongationIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.RequestOrderProlongationIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Create OrderProcessingRequest to prolongate order";
            }
        }
    }
}
