using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price
{
    [DataContract]
    public sealed class CopyPriceIdentity : OperationIdentityBase<CopyPriceIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CopyPriceIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Copy price";
            }
        }
    }
}
