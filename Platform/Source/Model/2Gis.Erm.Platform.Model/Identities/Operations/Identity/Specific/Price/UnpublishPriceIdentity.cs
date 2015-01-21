using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price
{
    public class UnpublishPriceIdentity : OperationIdentityBase<UnpublishPriceIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.UnpublishPriceIdentity; }
        }

        public override string Description
        {
            get { return "Unpublish price"; }
        }
    }
}