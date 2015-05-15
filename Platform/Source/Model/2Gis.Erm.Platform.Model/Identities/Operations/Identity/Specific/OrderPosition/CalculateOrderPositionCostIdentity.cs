using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition
{
    [DataContract]
    public sealed class CalculateOrderPositionCostIdentity : OperationIdentityBase<CalculateOrderPositionCostIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CalculateOrderPositionCostIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Calculate order position cost";
            }
        }
    }
}
