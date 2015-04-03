using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest
{
    // 2+ \Platform\Source\Model\2Gis.Erm.Platform.Model\Identities\Operations\Identity\Specific\OrderProcessingRequest
    [DataContract]
    public sealed class CreateOrderByRequestIdentity : OperationIdentityBase<CreateOrderByRequestIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CreateOrderByRequestIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Create order";
            }
        }
    }
}
