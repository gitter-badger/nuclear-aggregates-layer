using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    // 2+ \Platform\Source\Model\2Gis.Erm.Platform.Model\Identities\Operations\Identity\Specific\Order
    public sealed class ObtainDealForBizaccountOrderIdentity : OperationIdentityBase<ObtainDealForBizaccountOrderIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ObtainDealForBizaccountOrderIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "ObtainDealForBizaccountOrder";
            }
        }
    }
}