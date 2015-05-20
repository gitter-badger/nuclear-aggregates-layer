using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderProcessingRequest
{
    // TODO {d.ivanov, 04.12.2013}: 2+ \Platform\Source\Model\2Gis.Erm.Platform.Model\Identities\Operations\Identity\Specific\OrderProcessingRequest
    [DataContract]
    public sealed class GetOrderRequestMessagesIdentity : OperationIdentityBase<GetOrderRequestMessagesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.GetOrderRequestMessagesIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Get all messages for request";
            }
        }
    }
}
