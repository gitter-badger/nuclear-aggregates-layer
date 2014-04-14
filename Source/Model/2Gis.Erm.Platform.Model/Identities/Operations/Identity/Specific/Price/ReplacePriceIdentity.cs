using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price
{
    [DataContract]
    public class ReplacePriceIdentity : OperationIdentityBase<ReplacePriceIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ReplacePriceIdentity; }
        }

        public override string Description
        {
            get { return "Replace existing price"; }
        }
    }
}