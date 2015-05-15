using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price
{
    public class GetRatedPricesForCategoryIdentity : OperationIdentityBase<GetRatedPricesForCategoryIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.GetRatedPricesForCategoryIdentity; }
        }

        public override string Description
        {
            get { return "GetRatedPricesForCategoryIdentity"; }
        }
    }
}