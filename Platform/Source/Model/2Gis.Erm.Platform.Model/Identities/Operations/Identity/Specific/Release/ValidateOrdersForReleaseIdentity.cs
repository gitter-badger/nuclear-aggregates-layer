using System.Runtime.Serialization;

using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release
{
    [DataContract]
    public sealed class ValidateOrdersForReleaseIdentity : OperationIdentityBase<ValidateOrdersForReleaseIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ValidateOrdersForReleaseIdentity; }
        }

        public override string Description
        {
            get { return "Check orders involved to releasing process"; }
        }
    }
}