using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition
{
    [DataContract]
    public sealed class ValidateOrderPositionAdvertisementsIdentity : OperationIdentityBase<ValidateOrderPositionAdvertisementsIdentity>,
                                                                     INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ValidateOrderPositionAdvertisementsIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Validate order position advertisements";
            }
        }
    }
}
