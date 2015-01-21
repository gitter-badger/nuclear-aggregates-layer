using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    [DataContract]
    public sealed class CalculateOrderCostIdentity : OperationIdentityBase<CalculateOrderCostIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CalculateOrderCostIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Calculate order cost";
            }
        }
    }
}
