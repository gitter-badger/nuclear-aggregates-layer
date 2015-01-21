using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price
{
    public class PublishPriceIdentity : OperationIdentityBase<PublishPriceIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.PublishPriceIdentity; }
        }

        public override string Description
        {
            get { return "Publish price"; }
        }
    }
}